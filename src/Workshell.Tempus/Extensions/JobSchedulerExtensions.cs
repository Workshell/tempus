#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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

        public static Guid Schedule(this IJobScheduler scheduler, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule((context) =>
            {
                handler();

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule((context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(async (context) =>
            {
                await handler();
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule("@immediately", async (context) =>
            {
                await handler(context);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(when, (context) =>
            {
                handler();

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(when, (context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(when, async (context) =>
            {
                await handler();
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, DateTime when, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule($"@once {when:O}", async (context) =>
            {
                await handler(context);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(pattern, (context) =>
            {
                handler();

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(pattern, (context) =>
            {
                handler(context);

                return Task.FromResult(0);
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(pattern, async (context) =>
            {
                await handler();
            }, overlapHandling);
        }

        public static Guid Schedule(this IJobScheduler scheduler, string pattern, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow)
        {
            return scheduler.Schedule(pattern, async (context) =>
            {
                await handler(context);
            }, overlapHandling);
        }
        
        #endregion
    }
}
