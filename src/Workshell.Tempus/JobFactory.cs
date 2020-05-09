using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public class JobFactory : IJobFactory
    {
        #region Methods

        public virtual IJobScope CreateScope()
        {
            return new JobScope();
        }

        #endregion
    }
}
