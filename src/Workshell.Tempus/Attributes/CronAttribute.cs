using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CronAttribute : Attribute
    {
        public CronAttribute(string pattern)
        {
            Pattern = pattern;
        }

        #region Properties

        public string Pattern { get; }

        #endregion
    }
}
