using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public interface IScheduledJob
    {
        #region Properties

        Guid Id { get; }
        string Name { get; }
        string Pattern { get; }
        bool IsImmediately { get; }
        bool IsOnce { get; }
        bool IsAnonymous { get; }
        bool NoOverlap { get; }

        #endregion
    }
}
