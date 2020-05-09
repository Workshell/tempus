using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Workshell.Tempus
{
    public interface IActiveJob
    {
        #region Methods

        void Cancel();

        #endregion
        
        #region Properties

        long ExecutionId { get; }
        IScheduledJob Job { get; }
        DateTime Started { get; }
        TimeSpan Runtime { get; }
        CancellationToken Token { get; }

        #endregion
    }
}
