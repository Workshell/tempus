using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

namespace Workshell.Tempus.AspNetCore
{
    internal sealed class AspNetJobFactory : JobFactory
    {
        private readonly IServiceProvider _provider;

        public AspNetJobFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        #region Methods

        public override IJobScope CreateScope()
        {
            var scope = _provider.CreateScope();

            return new AspNetJobScope(scope);
        }

        #endregion
    }
}
