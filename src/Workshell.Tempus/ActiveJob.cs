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
using System.Threading;

namespace Workshell.Tempus
{
    internal sealed class ActiveJob : IDisposable, IActiveJob
    {
        private static long _executionId;

        private readonly CancellationTokenSource _cts;
        private volatile bool _disposed;

        static ActiveJob()
        {
            _executionId = 0;
        }
      
        internal ActiveJob(ScheduledJob job, CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _disposed = false;

            ExecutionId = Interlocked.Increment(ref _executionId);
            Job = job;
            Started = DateTime.UtcNow;
        }

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Cancel();
            _cts.Dispose();
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        #endregion

        #region Properties

        public long ExecutionId { get; }
        public IScheduledJob Job { get; }
        public DateTime Started { get; }
        public TimeSpan Runtime => (DateTime.UtcNow - Started);
        public CancellationToken Token => _cts.Token;

        #endregion
    }
}
