using System;

namespace TimeTracker.TimeTracking
{
    public class SessionLockEventArgs : EventArgs
    {
        public DateTime DateTime { get; set; }
    }
}
