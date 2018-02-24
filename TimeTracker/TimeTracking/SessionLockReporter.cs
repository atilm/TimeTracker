using Microsoft.Win32;
using System;
using System.ComponentModel.Composition;

namespace TimeTracker.TimeTracking
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SessionLockReporter
    {
        public virtual event EventHandler<SessionLockEventArgs> SessionLockedEvent;
        public virtual event EventHandler<SessionLockEventArgs> SessionUnlockedEvent;

        public SessionLockReporter()
        {
            SystemEvents.SessionSwitch +=
                new SessionSwitchEventHandler(OnSessionSwitched);
        }

        private void OnSessionSwitched(object sender, SessionSwitchEventArgs e)
        {
            var eventArgs = new SessionLockEventArgs { DateTime = DateTime.Now };

            if (IsUnlockingEvent(e))
                RaiseSessionUnlockedEvent(eventArgs);
            else
                RaiseSessionLockedEvent(eventArgs);
        }

        private bool IsUnlockingEvent(SessionSwitchEventArgs e)
        {
            return e.Reason.ToString().Contains("Unlock");
        }

        protected virtual void RaiseSessionLockedEvent(SessionLockEventArgs e)
        {
            SessionLockedEvent?.Invoke(this, e);
        }

        protected virtual void RaiseSessionUnlockedEvent(SessionLockEventArgs e)
        {
            SessionUnlockedEvent?.Invoke(this, e);
        }
    }
}
