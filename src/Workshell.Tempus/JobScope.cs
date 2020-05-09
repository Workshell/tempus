using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Workshell.Tempus
{
    public class JobScope : IJobScope
    {
        #region Methods

        public virtual void Dispose()
        {
        }

        public virtual IJob Create(Type type)
        {
            if (!(typeof(IJob).GetTypeInfo().IsAssignableFrom(type)))
            {
                throw new Exception("Type does not implement IJob interface.");
            }

            return (IJob)Activator.CreateInstance(type);
        }

        public IJob Create<T>() where T : IJob
        {
            return Create(typeof(T));
        }

        #endregion
    }
}
