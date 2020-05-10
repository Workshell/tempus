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

        static JobScheduler()
        {
            _locker = new object();
            _scheduler = null;
        }

        private JobScheduler(IJobFactory factory)
        {
            _jobs = new ScheduledJobs();
            _activeJobs = new ActiveJobs();
            _disposed = false;
            _cts = null;
            _timer = null;
            _factory = factory;
        }

        #region Static Methods

        public static IJobScheduler Create(IJobFactory factory = null)
        {
            lock (_locker)
            {
                if (_scheduler == null)
                {
                    _scheduler = new JobScheduler(factory ?? new JobFactory());
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
            lock (_locker)
            {
                if (_cts != null)
                {
                    return;
                }

                _cts = new CancellationTokenSource();
                _timer = new Timer(OnTimer, null, 0, 1000);
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                if (_cts == null)
                {
                    return;
                }

                _timer.Dispose();
                _cts.Cancel();
                _cts.Dispose();

                _timer = null;
                _cts = null;
            }
        }
        
        public Guid Schedule(Type type)
        {
            if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new Exception("Type is not an IJob.");
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

        private void OnTimer(object state)
        {
            var factory = Interlocked.Exchange(ref _factory, _factory);
            var jobs = _jobs.Next(DateTime.UtcNow);

            foreach (var job in jobs)
            {
                ExecuteJob(factory, job);
            }
        }

        private void ExecuteJob(IJobFactory factory, ScheduledJob job)
        {
            Task.Run(async () =>
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

        #endregion

        #region Properties

        public IJobFactory Factory
        {
            get => Interlocked.Exchange(ref _factory, _factory);
            set
            {
                var factory = value ?? new JobFactory();

                Interlocked.Exchange(ref _factory, factory);
            }
        }

        public IScheduledJobs ScheduledJobs => _jobs;
        public IActiveJobs ActiveJobs => _activeJobs;

        #endregion
    }
}
