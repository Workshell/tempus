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
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Workshell.Tempus.AspNetCore
{
    internal sealed class TempusBackgroundService : IDisposable, IHostedService
    {
        private readonly IJobScheduler _scheduler;
        private volatile bool _disposed;

        public TempusBackgroundService(IServiceProvider services, IJobScheduler scheduler)
        {
            var wrappers = services.GetServices<AspNetJobWrapper>();

            foreach(var wrapper in wrappers)
            {
                if (wrapper.Type != null)
                {
                    scheduler.Schedule(wrapper.Type);
                }
                else
                {
                    scheduler.Schedule(wrapper.Pattern, wrapper.Executor, wrapper.OverlapHandling);
                }
            }

            _scheduler = scheduler;
        }

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            //_scheduler.Dispose(); -- We get this from DI so probably don't want to dispose it
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler.Stop();

            return Task.CompletedTask;
        }

        #endregion
    }
}
