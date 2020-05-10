#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workshell.Tempus
{
    public sealed class JobScheduler : IJobScheduler
    {
        private static readonly object _locker;
        private static IJobScheduler _scheduler;

        private readonly ScheduledJobs _jobs;
        private readonly ActiveJobs _activeJobs;
        private volatile bool _disposed;
        private CancellationTokenSource _cts;
        private Timer _timer;
        private IJobFactory _factory;
        private IJobRunner _runner;

        static JobScheduler()
        {
            _locker = new object();
            _scheduler = null;
        }

        private JobScheduler(IJobFactory factory, IJobRunner runner)
        {
            _jobs = new ScheduledJobs();
            _activeJobs = new ActiveJobs();
            _disposed = false;
            _cts = null;
            _timer = null;
            _factory = factory;
            _runner = runner;
        }

        #region Static Methods

        public static IJobScheduler Create(IJobFactory factory = null, IJobRunner runner = null)
        {
            lock (_locker)
            {
                if (_scheduler == null)
                {
                    _scheduler = new JobScheduler(factory ?? new JobFactory(), runner ?? new JobRunner());
                }

                return _scheduler;
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Stop();
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        public void Start()
        {
            OnStarting();

            if (Volatile.Read(ref _cts) != null)
            {
                return;
            }

            lock (_locker)
            {
                _cts = new CancellationTokenSource();
                _timer = new Timer(TimerElapsed, null, 0, 1000);
            }

            OnStarted();
        }

        public void Stop(bool wait = false)
        {
            OnStopping();

            if (Volatile.Read(ref _cts) == null)
            {
                return;
            }

            lock (_locker)
            {
                _timer.Dispose(() => _timer = null);
                _cts.Dispose(() => _cts = null);
            }

            if (wait)
            {
                while (_activeJobs.Count > 0)
                {
                    Thread.Sleep(100);
                }
            }

            OnStopped();
        }
        
        public Guid Schedule(Type type)
        {
            if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new ArgumentException("Type is not an IJob.", nameof(type));
            }

            var job = _jobs.Add(type);

            return job.Id;
        }
        
        public Guid Schedule(string pattern, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var job = _jobs.Add(pattern, handler, overlapHandling);

            return job.Id;
        }

        public bool Unschedule(Guid id)
        {
            return _jobs.Remove(id);
        }

        private void TimerElapsed(object state)
        {
            var factory = Volatile.Read(ref _factory);
            var runner = Volatile.Read(ref _runner);
            var jobs = _jobs.Next(DateTime.UtcNow);

            foreach (var job in jobs)
            {
                ExecuteJob(factory, runner, job);
            }
        }

        private void ExecuteJob(IJobFactory factory, IJobRunner runner, ScheduledJob job)
        {
            if (!OnJobStarting(job))
            {
                return;
            }

            runner.Run(async () =>
            {
                if (job.OverlapHandling == OverlapHandling.Skip && _activeJobs.Contains(job))
                {
                    return;
                }

                Lock(job);

                var activeJob = new ActiveJob(job, _cts.Token);

                _activeJobs.Add(activeJob);

                try
                {
                    var context = new JobExecutionContext(this, activeJob.Token);

                    OnJobStarted(context);

                    try
                    {
                        if (!job.IsAnonymous)
                        {
                            using (var scope = factory.CreateScope())
                            {
                                var instance = scope.Create(job.Type);

                                await instance.ExecuteAsync(context);
                            }
                        }
                        else
                        {
                            await job.Handler(context);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (OnJobError(context, ex))
                        {
                            throw;
                        }
                    }

                    OnJobFinished(job);
                }
                finally
                {
                    _activeJobs.Remove(activeJob);
                    Unlock(job);
                }
            });
        }

        private void Lock(ScheduledJob job)
        {
            if (job.OverlapHandling != OverlapHandling.Wait)
            {
                return;
            }

            Monitor.Enter(job);
        }

        private void Unlock(ScheduledJob job)
        {
            if (job.OverlapHandling != OverlapHandling.Wait)
            {
                return;
            }

            Monitor.Exit(job);
        }

        private void OnStarting()
        {
            var handler = Volatile.Read(ref Starting);

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnStarted()
        {
            var handler = Volatile.Read(ref Started);

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnStopping()
        {
            var handler = Volatile.Read(ref Stopping);

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnStopped()
        {
            var handler = Volatile.Read(ref Stopped);

            handler?.Invoke(this, EventArgs.Empty);
        }

        private bool OnJobStarting(IScheduledJob job)
        {
            var startingHandler = Volatile.Read(ref JobStarting);

            if (startingHandler != null)
            {
                var startingArgs = new JobStartingEventArgs(job);

                startingHandler.Invoke(this, startingArgs);

                if (startingArgs.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnJobStarted(JobExecutionContext context)
        {
            var startedHandler = Volatile.Read(ref JobStarted);

            startedHandler?.Invoke(this, new JobStartedEventArgs(context));
        }

        private void OnJobFinished(IScheduledJob job)
        {
            var finishedHandler = Volatile.Read(ref JobFinished);

            finishedHandler?.Invoke(this, new JobEventArgs(job));
        }

        private bool OnJobError(JobExecutionContext context, Exception ex)
        {
            var errorHandler = Volatile.Read(ref JobError);

            if (errorHandler == null)
            {
                return false;
            }

            var errorArgs = new JobErrorEventArgs(context, ex);

            errorHandler.Invoke(this, errorArgs);

            if (errorArgs.Rethrow)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Properties

        public IJobFactory Factory
        {
            get => Volatile.Read<IJobFactory>(ref _factory);
            set
            {
                var factory = value ?? new JobFactory();

                Volatile.Write<IJobFactory>(ref _factory, factory);
            }
        }

        public IJobRunner Runner
        {
            get => Volatile.Read<IJobRunner>(ref _runner);
            set
            {
                var runner = value ?? new JobRunner();

                Volatile.Write<IJobRunner>(ref _runner, runner);
            }
        }

        public IScheduledJobs ScheduledJobs => _jobs;
        public IActiveJobs ActiveJobs => _activeJobs;

        #endregion

        #region Events

        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler Stopping;
        public event EventHandler Stopped;
        public event JobStartingEventHandler JobStarting;
        public event JobStartedEventHandler JobStarted;
        public event JobEventHandler JobFinished;
        public event JobErrorEventHandler JobError;

        #endregion
    }
}
