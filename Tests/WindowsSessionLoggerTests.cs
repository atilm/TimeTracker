using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TimeTracker.TimeTracking;

namespace Tests
{
    [TestClass]
    public class WindowsSessionLoggerTests
    {
        [TestMethod]
        public void Three_subsequent_lock_unlock_pairs_are_recorded_correctly()
        {
            mockLockReporter = new Mock<SessionLockReporter>();
            var sessionLogger = new WindowsSessionLogger(mockLockReporter.Object);

            RaiseLockEvent(new DateTime(2018, 2, 24, 8, 30, 0));
            RaiseUnlockEvent(new DateTime(2018, 2, 24, 9, 0, 0));

            RaiseLockEvent(new DateTime(2018, 2, 24, 10, 15, 0));
            RaiseUnlockEvent(new DateTime(2018, 2, 24, 10, 45, 0));

            RaiseLockEvent(new DateTime(2018, 2, 24, 13, 0, 0));
            RaiseUnlockEvent(new DateTime(2018, 2, 24, 13, 20, 0));

            Assert.AreEqual(3, sessionLogger.Records.Count);

            Assert.AreEqual(new DateTime(2018, 2, 24, 8, 30, 0), sessionLogger.Records[2].LockTime);
            Assert.AreEqual(new DateTime(2018, 2, 24, 9, 0, 0), sessionLogger.Records[2].UnlockTime);
            Assert.AreEqual(new TimeSpan(0, 30, 0), sessionLogger.Records[2].LockDuration);

            Assert.AreEqual(new DateTime(2018, 2, 24, 10, 15, 0), sessionLogger.Records[1].LockTime);
            Assert.AreEqual(new DateTime(2018, 2, 24, 10, 45, 0), sessionLogger.Records[1].UnlockTime);
            Assert.AreEqual(new TimeSpan(0, 30, 0), sessionLogger.Records[1].LockDuration);

            Assert.AreEqual(new DateTime(2018, 2, 24, 13, 0, 0), sessionLogger.Records[0].LockTime);
            Assert.AreEqual(new DateTime(2018, 2, 24, 13, 20, 0), sessionLogger.Records[0].UnlockTime);
            Assert.AreEqual(new TimeSpan(0, 20, 0), sessionLogger.Records[0].LockDuration);
        }

        private void RaiseLockEvent(DateTime dateTime)
        {
            mockLockReporter.Raise(m => m.SessionLockedEvent += null,
                new SessionLockEventArgs
                {
                    DateTime = dateTime
                });
        }

        private void RaiseUnlockEvent(DateTime dateTime)
        {
            mockLockReporter.Raise(m => m.SessionUnlockedEvent += null,
                new SessionLockEventArgs
                {
                    DateTime = dateTime
                });
        }

        private Mock<SessionLockReporter> mockLockReporter;
    }
}
