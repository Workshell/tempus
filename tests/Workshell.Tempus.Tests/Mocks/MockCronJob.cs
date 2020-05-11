using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus.Tests.Mocks
{
    [Cron("*/10 * * * * *")]
    [Overlap(OverlapHandling.Wait)]
    public sealed class MockCronJob : IJob
    {
        public Task ExecuteAsync(JobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
