using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    public interface IJobScope : IDisposable
    {
        #region Methods

        IJob Create(Type type);
        IJob Create<T>() where T : IJob;

        #endregion
    }
}
