using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Workshell.Tempus.Tests.Mocks;

namespace Workshell.Tempus.Tests
{
    [TestFixture]
    public sealed class ScheduledJobTests
    {
        private Func<JobExecutionContext, Task> _dummyHandler;

        [OneTimeSetUp]
        public void SetUp()
        {
            _dummyHandler = (context) => Task.CompletedTask;
        }

        [Test]
        public void Constructor_With_Invalid_Type_Throws_Exception()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new ScheduledJob(typeof(String), null);
            });
        }

        [Test]
        public void Constructor_With_Invalid_Pattern_Throws_Exception()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new ScheduledJob(string.Empty, null, OverlapHandling.Allow, null);
            });
        }

        [Test]
        public void Constructor_With_Invalid_Handler_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ScheduledJob("@immediately", null, OverlapHandling.Allow, null);
            });
        }

        /* Immediate Jobs */

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.AreEqual("@immediately", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.AreEqual(typeof(MockImmediateJob), scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.IsNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.IsTrue(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.IsFalse(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Typed_Immediate_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob), null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }
        
        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.AreEqual("@immediately", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNull(scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNotNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsTrue(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsTrue(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Wait, null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Untyped_Immediate_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow, null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }

        /* One-time Jobs */

        [Test]
        public void Constructor_With_Typed_Once_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);
            var startsWith = scheduledJob.Pattern.StartsWith("@once ");
                
            Assert.IsTrue(startsWith);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.AreEqual(typeof(MockOnceJob), scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.IsNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.IsFalse(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.IsTrue(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.IsFalse(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Typed_Once_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Typed_Once_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob(typeof(MockOnceJob), null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }
        
        [Test]
        public void Constructor_With_Untyped_Once_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.AreEqual("@once 2400-12-25T00:00:00Z", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNull(scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNotNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsFalse(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsTrue(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsTrue(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Untyped_Once_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Wait, null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Untyped_Once_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob("@once 2400-12-25T00:00:00Z", _dummyHandler, OverlapHandling.Allow, null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }

        /* Cron Jobs */

        [Test]
        public void Constructor_With_Typed_Cron_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);
   
            Assert.AreEqual("*/10 * * * * *", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.AreEqual(typeof(MockCronJob), scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.IsNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.IsFalse(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.IsFalse(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Typed_Cron_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Typed_Cron_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 10, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob(typeof(MockCronJob), null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }
        
        [Test]
        public void Constructor_With_Untyped_Cron_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.AreEqual("*/10 * * * * *", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNull(scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsNotNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsFalse(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);

            Assert.IsTrue(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Untyped_Cron_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Wait, null);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Untyped_Cron_Job_Is_True()
        {
            var christmas = new DateTime(2400, 12, 25, 0, 0, 10, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob("*/10 * * * * *", _dummyHandler, OverlapHandling.Allow, null);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }
    }
}
