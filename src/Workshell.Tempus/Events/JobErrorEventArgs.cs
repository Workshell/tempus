using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class JobErrorEventArgs : EventArgs
    {
        internal JobErrorEventArgs(JobExecutionContext context, Exception exception)
        {
            Context = context;
            Exception = exception;
            Rethrow = false;
        }

        #region Properties

        public JobExecutionContext Context { get; }
        public Exception Exception { get; }
        public bool Rethrow { get; set; }

        #endregion
    }
}
