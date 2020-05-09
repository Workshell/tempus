using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus
{
    public interface IJob
    {
        #region Methods

        Task ExecuteAsync(JobExecutionContext context);

        #endregion
    }
}
