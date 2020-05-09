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

namespace Workshell.Tempus.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Methods

        public static IServiceCollection AddTempus(this IServiceCollection services, bool backgroundService = true)
        {
            services.AddSingleton<IJobFactory, AspNetJobFactory>();
            services.AddSingleton<IJobScheduler>(provider =>
            {
                var scheduler = JobScheduler.Create(provider.GetService<IJobFactory>());

                return scheduler;
            });

            if (backgroundService)
            {
                services.AddSingleton<IHostedService, TempusBackgroundService>();
            }

            return services;
        }

        public static IServiceCollection AddTempusJob<T>(this IServiceCollection services) where T : class, IJob
        {
            services.AddScoped(typeof(T));
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper(typeof(T)));

            return services;
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Action handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Func<Task> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper("@immediately", handler, noOverlap));

            return services;
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Action handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, when, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, when, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Func<Task> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, when, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, DateTime when, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper($"@once {when:O}", handler, noOverlap));

            return services;
        }
        
        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Action handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler();

                return Task.CompletedTask;
            };

            return AddTempusJob(services, pattern, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Action<JobExecutionContext> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) =>
            {
                handler(context);

                return Task.CompletedTask;
            };

            return AddTempusJob(services, pattern, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Func<Task> handler, bool noOverlap = false)
        {
            Func<JobExecutionContext, Task> funcHandler = (context) => handler();

            return AddTempusJob(services, pattern, funcHandler, noOverlap);
        }

        public static IServiceCollection AddTempusJob(this IServiceCollection services, string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            services.AddTransient<AspNetJobWrapper>(provider => new AspNetJobWrapper(pattern, handler, noOverlap));

            return services;
        }

        #endregion
    }
}
