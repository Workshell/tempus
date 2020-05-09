using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Workshell.Tempus
{
    public sealed class JobExecutionContext
    {
        private readonly CancellationToken _cancellationToken;

        internal JobExecutionContext(IJobScheduler scheduler, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            Scheduler = scheduler;
        }

        #region Properties

        public IJobScheduler Scheduler { get; }
        public bool IsCancellationRequested => _cancellationToken.IsCancellationRequested;

        #endregion
    }
}
