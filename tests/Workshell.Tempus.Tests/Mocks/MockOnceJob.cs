using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus.Tests.Mocks
{
    [Once("2020-12-25T00:00:00Z")]
    [Overlap(OverlapHandling.Wait)]
    public sealed class MockOnceJob : IJob
    {
        public Task ExecuteAsync(JobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
