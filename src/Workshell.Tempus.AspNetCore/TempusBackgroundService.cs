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
                    scheduler.Schedule(wrapper.Pattern, wrapper.Executor, wrapper.NoOverlap);
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
