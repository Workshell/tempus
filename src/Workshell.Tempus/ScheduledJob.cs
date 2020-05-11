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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Cronos;

namespace Workshell.Tempus
{
    internal sealed class ScheduledJob : IScheduledJob
    {
        private readonly CronExpression _cron;
        private DateTime? _next;

        public ScheduledJob(Type type)
        {
            if (!Utils.SupportsJob(type))
            {
                throw new ArgumentException("Type does not support IJob.", nameof(type));
            }

            var pattern = GetPattern(type);

            if (pattern == "@immediately")
            {
                _cron = null;
                _next = DateTime.UtcNow;
            }
            else if (pattern.StartsWith("@once "))
            {
                var value = pattern.Remove(0, 6);

                _cron = null;
                _next = DateTime.Parse(value);
            }
            else
            {
                _cron = CronExpression.Parse(pattern, CronFormat.IncludeSeconds);
                _next = _cron.GetNextOccurrence(DateTime.UtcNow);
            }

            Id = Guid.NewGuid();
            Pattern = pattern;
            Type = type;
            Handler = null;
            IsImmediately = (pattern == "@immediately");
            IsOnce = pattern.StartsWith("@once ");
            IsAnonymous = false;
            OverlapHandling = GetOverlapHandling(type);
        }

        public ScheduledJob(string pattern, Func<JobExecutionContext, Task> handler, OverlapHandling overlapHandling)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("No pattern was specified.", nameof(pattern));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler), "No handler was specified.");
            }

            if (pattern == "@immediately")
            {
                _cron = null;
                _next = DateTime.UtcNow;
            }
            else if (pattern.StartsWith("@once "))
            {
                var value = pattern.Remove(0, 6);

                _cron = null;
                _next = DateTime.Parse(value);
            }
            else
            {
                _cron = CronExpression.Parse(pattern, CronFormat.IncludeSeconds);
                _next = _cron.GetNextOccurrence(DateTime.UtcNow);
            }

            Id = Guid.NewGuid();
            Pattern = pattern;
            Type = null;
            Handler = handler;
            IsImmediately = (pattern == "@immediately");
            IsOnce = pattern.StartsWith("@once ");
            IsAnonymous = true;
            OverlapHandling = overlapHandling;
        }

        #region Methods

        public bool? NeedsExecuting(DateTime current)
        {
            if (_next == null)
            {
                return null;
            }

            if (_next <= current)
            {
                _next = _cron?.GetNextOccurrence(_next.Value);

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            var results = new List<string>();

            results.Add($"Id: {Id:D}");

            if (IsAnonymous)
            {
                results.Add("Type: Anonymous");
            }
            else
            {
                results.Add($"Type: {Type.FullName}");
            }

            if (IsImmediately)
            {
                results.Add("When: Immediately");
            }
            else if (IsOnce)
            {
                results.Add($"When: Once ({_next})");
            }
            else
            {
                results.Add($"When: {Pattern} ({_next})");
            }

            return string.Join("; ", results);
        }

        private OverlapHandling GetOverlapHandling(Type type)
        {
            if (type == null)
            {
                return OverlapHandling.Allow;
            }

            var attributes = type.GetTypeInfo().GetCustomAttributes();
            var attribute = (OverlapAttribute)attributes.FirstOrDefault(_ => _ is OverlapAttribute);

            if (attribute == null)
            {
                return OverlapHandling.Allow;
            }

            return attribute.Handling;
        }

        private string GetPattern(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }

            var attributes = type.GetTypeInfo().GetCustomAttributes();

            var cronAttr = (CronAttribute)attributes.FirstOrDefault(_ => _ is CronAttribute);
            var onceAttr = (OnceAttribute)attributes.FirstOrDefault(_ => _ is OnceAttribute);

            if (cronAttr == null && onceAttr == null)
            {
                return "@immediately";
            }

            if (cronAttr != null && onceAttr != null)
            {
                throw new TempusException("Cannot have both a cron and a once attribute, they're mutually exclusive.");
            }

            if (onceAttr != null)
            {
                return $"@once {onceAttr.When:O}";
            }

            return cronAttr.Pattern;
        }

        #endregion

        #region Properties

        public Type Type { get; }
        public Func<JobExecutionContext, Task> Handler { get; }

        public Guid Id { get; }
        public string Name => (IsAnonymous ? "Anonymous" : Type.FullName);
        public string Pattern { get; }
        public bool IsImmediately { get; }
        public bool IsOnce { get; }
        public bool IsAnonymous { get; }
        public OverlapHandling OverlapHandling { get; }

        #endregion
    }
}
