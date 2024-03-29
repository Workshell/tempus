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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Workshell.Tempus.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        #region Methods

        public static IServiceCollection AddTempus(this IServiceCollection services, Action<TempusOptions> configure = null)
        {
            var options = new TempusOptions();

            configure?.Invoke(options);

            if (options.Factory == null)
            {
                services.AddSingleton<IJobFactory, AspNetJobFactory>();
            }
            else
            {
                services.AddSingleton<IJobFactory>(options.Factory);
            }

            if (options.Runner == null)
            {
                services.AddSingleton<IJobRunner, JobRunner>();
            }
            else
            {
                services.AddSingleton<IJobRunner>(options.Runner);
            }

            services.AddSingleton<IJobScheduler>(provider =>
            {
                var scheduler = JobScheduler.Create(provider.GetService<IJobFactory>(), provider.GetService<IJobRunner>());

                return scheduler;
            });

            if (options.RegisterBackgroundService)
            {
                services.AddSingleton<IHostedService, TempusBackgroundService>();
            }

            return services;
        }

        public static IServiceCollection AddTempusJob<T>(this IServiceCollection services, object state = null) where T : class, IJob
        {
            services.AddScoped(typeof(T));
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper(typeof(T), state));

            return services;
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow, object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper("@immediately", handler, overlapHandling, state));

            return services;
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, when, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, when, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, when, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper($"@once {when:O}", handler, overlapHandling, state));

            return services;
        }
        
        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Action handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, pattern, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Action<JobExecutionContext> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, pattern, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Func<Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, pattern, funcHandler, overlapHandling, state);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling = OverlapHandling.Allow,
            object state = null)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper(pattern, handler, overlapHandling, state));

            return services;
        }

        #endregion
    }
}

