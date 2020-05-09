using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public interface IScheduledJobs
    {
        #region Methods

        IScheduledJob[] ToArray();

        #endregion

        #region Properties

        int Count { get; }
        IScheduledJob this[Guid id] { get; }

        #endregion
    }
}
