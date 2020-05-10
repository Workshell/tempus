#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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

        public AspNetJobWrapper(string pattern, Func<JobExecutionContext, Task> executor, OverlapHandling overlapHandling)
        {
            Pattern = pattern;
            Type = null;
            Executor = executor;
            OverlapHandling = overlapHandling;
        }

        #region Properties

        public string Pattern { get; }
        public Type Type { get; }
        public Func<JobExecutionContext, Task> Executor { get; }
        public OverlapHandling OverlapHandling { get; }

        #endregion
    }
}
