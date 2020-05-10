using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class JobStartedEventArgs : EventArgs
    {
        internal JobStartedEventArgs(JobExecutionContext context)
        {
            Context = context;
        }

        #region Properties

        public JobExecutionContext Context { get; }

        #endregion
    }
}
