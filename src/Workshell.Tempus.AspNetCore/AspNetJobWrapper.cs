using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.Tempus.AspNetCore
{
    internal sealed class AspNetJobWrapper
    {
        public AspNetJobWrapper(Type type)
        {
            if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(type))
            {
                throw new ArgumentException("Specified type does not support IJob.");
            }

            Pattern = null;
            Type = type;
            Executor = null;
        }

        public AspNetJobWrapper(string pattern, Func<JobExecutionContext, Task> executor, bool noOverlap)
        {
            Pattern = pattern;
            Type = null;
            Executor = executor;
            NoOverlap = noOverlap;
        }

        #region Properties

        public string Pattern { get; }
        public Type Type { get; }
        public Func<JobExecutionContext, Task> Executor { get; }
        public bool NoOverlap { get; }

        #endregion
    }
}
