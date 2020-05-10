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

        /* Immediate Jobs */

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.AreEqual("@immediately", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.AreEqual(typeof(MockImmediateJob), scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.IsNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.IsTrue(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.IsFalse(scheduledJob.IsAnonymous);
        }

        [Test]
        public void Constructor_With_Typed_Immediate_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }

        [Test]
        public void NeedsExecuting_With_Typed_Immediate_Job_Is_True()
        {
            var christmas = new DateTime(2020, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob(typeof(MockImmediateJob));
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }
        
        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Pattern_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.AreEqual("@immediately", scheduledJob.Pattern);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Type_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.IsNull(scheduledJob.Type);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_Handler_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.IsNotNull(scheduledJob.Handler);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsImmediately_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.IsTrue(scheduledJob.IsImmediately);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsOnce_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.IsFalse(scheduledJob.IsOnce);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_IsAnonymous_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);

            Assert.IsTrue(scheduledJob.IsAnonymous);
        }

        [Test]
        public void NeedsExecuting_With_Untyped_Immediate_Job_Is_True()
        {
            var christmas = new DateTime(2020, 12, 25, 0, 0, 0, DateTimeKind.Utc);
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Allow);
            var next = scheduledJob.NeedsExecuting(christmas);

            Assert.IsTrue(next ?? false);
        }

        [Test]
        public void Constructor_With_Untyped_Immediate_Job_OverlapHandling_Is_Correct()
        {
            var scheduledJob = new ScheduledJob("@immediately", _dummyHandler, OverlapHandling.Wait);

            Assert.AreEqual(OverlapHandling.Wait, scheduledJob.OverlapHandling);
        }
    }
}
