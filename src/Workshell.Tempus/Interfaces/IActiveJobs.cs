using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public interface IActiveJobs
    {
        #region Methods

        IActiveJob[] ToArray(ActiveJobOrder order = ActiveJobOrder.StartedOldest);

        #endregion

        #region Properties

        int Count { get; }
        IActiveJob this[long executionId] { get; }

        #endregion
    }
}
