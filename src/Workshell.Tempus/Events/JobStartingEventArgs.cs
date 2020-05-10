using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class JobStartingEventArgs : JobEventArgs
    {
        internal JobStartingEventArgs(IScheduledJob job) : base(job)
        {
            Cancel = false;
        }

        #region Properties

        public bool Cancel { get; set; }

        #endregion
    }
}
