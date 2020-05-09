using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OnceAttribute : Attribute
    {
        public OnceAttribute(string when)
        {
            When = DateTime.Parse(when);
        }

        #region Properties

        public DateTime When { get; }

        #endregion
    }
}
