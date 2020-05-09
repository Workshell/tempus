using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.Tempus
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NoOverlapAttribute : Attribute
    {
    }
}
