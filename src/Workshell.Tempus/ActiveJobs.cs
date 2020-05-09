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
using System.Linq;
using System.Text;
using System.Threading;

namespace Workshell.Tempus
{
    internal sealed class ActiveJobs : IActiveJobs
    {
        private readonly ReaderWriterLockSlim _locker;
        private readonly HashSet<ActiveJob> _jobs;

        internal ActiveJobs()
        {
            _locker = new ReaderWriterLockSlim();
            _jobs = new HashSet<ActiveJob>();
        }

        #region Methods

        public bool Add(ActiveJob job)
        {
            _locker.EnterWriteLock();

            try
            {
                return _jobs.Add(job);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public bool Remove(long executionId)
        {
            _locker.EnterUpgradeableReadLock();

            try
            {
                foreach (var job in _jobs)
                {
                    if (job.ExecutionId == executionId)
                    {
                        _locker.EnterWriteLock();

                        try
                        {
                            _jobs.Remove(job);

                            return true;
                        }
                        finally
                        {
                            _locker.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }

            return false;
        }

        public bool Remove(IActiveJob job)
        {
            return Remove(job?.ExecutionId ?? -1);
        }

        public IActiveJob[] ToArray(ActiveJobOrder order = ActiveJobOrder.StartedOldest)
        {
            _locker.EnterReadLock();

            try
            {
                IEnumerable<ActiveJob> results;

                switch (order)
                {
                    case ActiveJobOrder.StartedOldest:
                        results = _jobs.OrderBy(_ => _.Started);
                        break;
                    case ActiveJobOrder.StartedYoungest:
                        results = _jobs.OrderByDescending(_ => _.Started);
                        break;
                    case ActiveJobOrder.RuntimeLongest:
                        results = _jobs.OrderByDescending(_ => _.Runtime);
                        break;
                    case ActiveJobOrder.RuntimeShortest:
                        results = _jobs.OrderBy(_ => _.Runtime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(order), order, null);
                }

                return results.Cast<IActiveJob>().ToArray();
            }
            finally
            {
                _locker.ExitReadLock();
            }
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

        public IActiveJob this[long executionId]
        {
            get
            {
                _locker.EnterReadLock();

                try
                {
                    var job = _jobs.FirstOrDefault(_ => _.ExecutionId == executionId);

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
