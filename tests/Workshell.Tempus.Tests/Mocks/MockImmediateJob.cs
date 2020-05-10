using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus.Tests.Mocks
{
    [Overlap(OverlapHandling.Wait)]
    public sealed class MockImmediateJob : IJob
    {
        public Task ExecuteAsync(JobExecutionContext context)
        {
            return Task.CompletedTask;
        }           
    }
}
