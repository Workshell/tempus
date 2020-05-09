using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus
{
    public static class JobSchedulerExtensions
    {
        #region Methods

        public static Guid Schedule<T>(this IJobScheduler scheduler) where T : IJob
        {
            return scheduler.Schedule(typeof(T));
        }

        public static Guid Schedule(this IJobScheduler scheduler, Action handler, bool noOverlap = false)
        {
            return scheduler.Schedule((context) =>
            {
                handler();

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            return scheduler.Schedule((context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Func<Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(async (context) =>
            {
                await handler();
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule("@immediately", async (context) =>
            {
                await handler(context);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Action handler, bool noOverlap = false)
        {
            return scheduler.Schedule(when, (context) =>
            {
                handler();

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(when, (context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Func<Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(when, async (context) =>
            {
                await handler();
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule($"@once {when:O}", async (context) =>
            {
                await handler(context);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Action handler, bool noOverlap = false)
        {
            return scheduler.Schedule(pattern, (context) =>
            {
                handler();

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(pattern, (context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Func<Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(pattern, async (context) =>
            {
                await handler();
            }, noOverlap);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            return scheduler.Schedule(pattern, async (context) =>
            {
                await handler(context);
            }, noOverlap);
        }
        
        #endregion
    }
}
