using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus
{
    public interface IJobScheduler : IDisposable
    {
        #region Methods

        void Start();
        void Stop();
        Guid Schedule(Type type);
        Guid Schedule(string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false);
        bool Unschedule(Guid id);

        #endregion

        #region Properties

        IJobFactory Factory { get; set; }
        IScheduledJobs ScheduledJobs { get; }
        IActiveJobs ActiveJobs { get; }

        #endregion
    }
}
