using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Workshell.Tempus
{
    internal sealed class ActiveJob : IDisposable, IActiveJob
    {
        private static long _executionId;

        private readonly CancellationTokenSource _cts;
        private volatile bool _disposed;

        static ActiveJob()
        {
            _executionId = 0;
        }
      
        internal ActiveJob(ScheduledJob job, CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _disposed = false;

            ExecutionId = Interlocked.Increment(ref _executionId);
            Job = job;
            Started = DateTime.UtcNow;
        }

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Cancel();
            _cts.Dispose();
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        #endregion

        #region Properties

        public long ExecutionId { get; }
        public IScheduledJob Job { get; }
        public DateTime Started { get; }
        public TimeSpan Runtime => (DateTime.UtcNow - Started);
        public CancellationToken Token => _cts.Token;

        #endregion
    }
}
