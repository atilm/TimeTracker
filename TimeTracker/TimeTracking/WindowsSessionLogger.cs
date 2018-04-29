using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace TimeTracker.TimeTracking
{
    [Export(typeof(WindowsSessionLogger))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WindowsSessionLogger
    {
        public WindowsSessionLogger() : this(null)
        {
        }

        [ImportingConstructor]
        public WindowsSessionLogger(SessionLockReporter sessionLockReporter)
        {
            SetSessionLockReporter(sessionLockReporter);

            Records = new ObservableCollection<SessionLockRecord>();
        }

        virtual public ObservableCollection<SessionLockRecord> Records { get; set; }

        private void SetSessionLockReporter(SessionLockReporter sessionLockReporter)
        {
            if (sessionLockReporter == null)
                return;

            this.sessionLockReporter = sessionLockReporter;
            this.sessionLockReporter.SessionLockedEvent += LogLockedEvent;
            this.sessionLockReporter.SessionUnlockedEvent += LogUnlockedEvent;
        }

        private void LogLockedEvent(object sender, SessionLockEventArgs e)
        {
            Records.Insert(0, new SessionLockRecord
            {
                LockTime = e.DateTime
            });
        }

        private void LogUnlockedEvent(object sender, SessionLockEventArgs e)
        {
            if (Records.Count == 0)
                return;

            var openRecord = Records.First();

            if (openRecord.LockTime == null)
                return;

            openRecord.UnlockTime = e.DateTime;
        }

        private SessionLockReporter sessionLockReporter;
    }
}
