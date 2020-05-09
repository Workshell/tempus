using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workshell.Tempus
{
    internal sealed class ScheduledJobs : IScheduledJobs
    {
        private readonly ReaderWriterLockSlim _locker;
        private readonly List<ScheduledJob> _jobs;

        public ScheduledJobs()
        {
            _locker = new ReaderWriterLockSlim();
            _jobs = new List<ScheduledJob>();
        }

        #region Methods

        public IScheduledJob Add(Type type)
        {
            _locker.EnterUpgradeableReadLock();

            try
            {
                var existingJob = _jobs.Any(_ => _.Type == type);

                if (existingJob)
                {
                    throw new TempusException("The job type is already scheduled.");
                }

                _locker.EnterWriteLock();

                try
                {
                    var job = new ScheduledJob(type);

                    _jobs.Add(job);

                    return job;
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
        }

        public IScheduledJob Add(string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            var job = new ScheduledJob(pattern, handler, noOverlap);

            _locker.EnterWriteLock();

            try
            {
                _jobs.Add(job);
            }
            finally
            {
                _locker.ExitWriteLock();
            }

            return job;
        }

        public bool Remove(Guid id)
        {
            _locker.EnterUpgradeableReadLock();

            try
            {
                var job = _jobs.FirstOrDefault(_ => _.Id == id);

                if (job != null)
                {
                    _locker.EnterWriteLock();

                    try
                    {
                        return _jobs.Remove(job);
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }

                return false;
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
        }

        public IScheduledJob[] ToArray()
        {
            _locker.EnterReadLock();

            try
            {
                return _jobs.Cast<IScheduledJob>().ToArray();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public ScheduledJob[] Next(Action<Guid[]> removedCallback = null)
        {
            return Next(DateTime.UtcNow);
        }

        public ScheduledJob[] Next(DateTime until)
        {
            var results = new List<ScheduledJob>(_jobs.Count);

            _locker.EnterUpgradeableReadLock();

            try
            {
                var expired = new List<ScheduledJob>(_jobs.Count);

                foreach (var job in _jobs)
                {
                    var needsExecuting = job.NeedsExecuting(until);

                    if (needsExecuting == null)
                    {
                        expired.Add(job);
                    }
                    else if (needsExecuting.Value)
                    {
                        results.Add(job);
                    }
                }

                if (expired.Count > 0)
                {
                    _locker.EnterWriteLock();

                    try
                    {
                        foreach (var job in expired)
                        {
                            _jobs.Remove(job);
                        }
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                _locker.EnterReadLock();

                try
                {
                    return _jobs.Count;
                }
                finally
                {
                    _locker.ExitReadLock();
                }
            }
        }

        public IScheduledJob this[Guid id]
        {
            get
            {
                _locker.EnterReadLock();

                try
                {
                    var job = _jobs.FirstOrDefault(_ => _.Id == id);

                    return job;
                }
                finally
                {
                    _locker.ExitReadLock();
                }
            }
        }

        #endregion
    }
}
