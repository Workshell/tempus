using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class JobEventArgs : EventArgs
    {
        internal JobEventArgs(IScheduledJob job)
        {
            Job = job;
        }

        #region Properties

        public IScheduledJob Job { get; }

        #endregion
    }
}
