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
    public delegate void JobStartingEventHandler(object sender, JobStartingEventArgs e);
    public delegate void JobStartedEventHandler(object sender, JobStartedEventArgs e);
    public delegate void JobEventHandler(object sender, JobEventArgs e);
    public delegate void JobErrorEventHandler(object sender, JobErrorEventArgs e);

    public interface IJobScheduler : IDisposable
    {
        #region Methods

        void Start();
        void Stop(bool wait = false);
        Guid Schedule(Type type, object state = null);
        Guid Schedule(string pattern, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow, object state = null);
        bool Unschedule(Guid id);

        #endregion

        #region Properties

        IJobFactory Factory { get; set; }
        IJobRunner Runner { get; set; }
        IScheduledJobs ScheduledJobs { get; }
        IActiveJobs ActiveJobs { get; }

        #endregion

        #region Events

        event EventHandler Starting;
        event EventHandler Started;
        event EventHandler Stopping;
        event EventHandler Stopped;
        event JobStartingEventHandler JobStarting;
        event JobStartedEventHandler JobStarted;
        event JobEventHandler JobFinished;
        event JobErrorEventHandler JobError;

        #endregion
    }
}
