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

        private readonly object _locker;
        private readonly ScheduledJobs _jobs;
        private readonly ActiveJobs _activeJobs;
        private volatile bool _disposed;
        private CancellationTokenSource _cts;
        private Timer _timer;
        private IJobFactory _factory;

        private JobScheduler(IJobFactory factory)
        {
            _locker = new object();
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
            return new JobScheduler(factory ?? new JobFactory());
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
        
        public Guid Schedule(string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var job = _jobs.Add(pattern, handler, noOverlap);

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
                var activeJob = new ActiveJob(job, _cts.Token);

                _activeJobs.Add(activeJob);
                Lock(job);

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
                    Unlock(job);
                    _activeJobs.Remove(activeJob);
                }
            });
        }

        private void Lock(ScheduledJob job)
        {
            if (!job.NoOverlap)
            {
                return;
            }

            Monitor.Enter(job);
        }

        private void Unlock(ScheduledJob job)
        {
            if (!job.NoOverlap)
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
