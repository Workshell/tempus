using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public interface IJobFactory
    {
        #region Methods

        IJobScope CreateScope();

        #endregion
    }
}
