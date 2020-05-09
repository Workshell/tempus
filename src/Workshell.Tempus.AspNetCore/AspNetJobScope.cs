using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

namespace Workshell.Tempus.AspNetCore
{
    internal sealed class AspNetJobScope : JobScope
    {
        private readonly IServiceScope _scope;
        private volatile bool _disposed;

        public AspNetJobScope(IServiceScope scope)
        {
            _scope = scope;
            _disposed = false;
        }

        #region Methods

        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _scope.Dispose();
            GC.SuppressFinalize(this);

            _disposed = true;
        }

        public override IJob Create(Type type)
        {
            if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new ArgumentException("Specified type does not support IJob.");
            }

            var job = (IJob)_scope.ServiceProvider.GetService(type);

            return job;
        }

        #endregion
    }
}
