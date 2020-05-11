using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Workshell.Tempus
{
    internal static class Utils
    {
        #region Methods

        public static bool SupportsJob(Type type)
        {
            return typeof(IJob).GetTypeInfo().IsAssignableFrom(type);
        }

        #endregion
    }
}
