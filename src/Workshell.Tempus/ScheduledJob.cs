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
            var pattern = GetPattern(type);

            if (pattern == "@immediately")
            {
                _cron = null;
                _next = DateTime.UtcNow;
            }
            else if (pattern.StartsWith("@once "))
            {
                var value = pattern.Substring(0, 6);

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
            NoOverlap = HasNoOverlap(type);
        }

        public ScheduledJob(string pattern, Func<JobExecutionContext, Task> handler, bool noOverlap = false)
        {
            if (pattern == "@immediately")
            {
                _cron = null;
                _next = DateTime.UtcNow;
            }
            else if (pattern.StartsWith("@once "))
            {
                var value = pattern.Substring(0, 6);

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
            NoOverlap = noOverlap;
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

        private bool HasNoOverlap(Type type)
        {
            if (type == null)
            {
                return false;
            }

            var attributes = type.GetTypeInfo().GetCustomAttributes();
            var attribute = attributes.FirstOrDefault(_ => _ is NoOverlapAttribute);

            if (attribute == null)
            {
                return false;
            }

            return true;
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
        public bool NoOverlap { get; }

        #endregion
    }
}
