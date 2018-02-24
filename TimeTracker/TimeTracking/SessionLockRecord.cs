using Prism.Mvvm;
using System;

namespace TimeTracker.TimeTracking
{
    public class SessionLockRecord : BindableBase
    {
        public DateTime? LockTime
        {
            get { return lockTime; }
            set { SetProperty(ref lockTime, value); }
        }

        public DateTime? UnlockTime
        {
            get { return unlockTime; }
            set
            {
                SetProperty(ref unlockTime, value);
                RaisePropertyChanged(nameof(LockDuration));
            }
        }

        public TimeSpan? LockDuration
        {
            get
            {
                if (UnlockTime == null || LockTime == null)
                    return null;

                return UnlockTime - LockTime;
            }
        }

        private DateTime? lockTime = null;
        private DateTime? unlockTime = null;
    }
}
