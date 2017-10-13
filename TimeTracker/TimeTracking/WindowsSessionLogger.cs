using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace TimeTracker.TimeTracking
{
    [Export(typeof(WindowsSessionLogger))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WindowsSessionLogger
    {
        public WindowsSessionLogger()
        {
            Records = new ObservableCollection<SessionLoggingRecord>();

            SystemEvents.SessionSwitch += 
                new SessionSwitchEventHandler(OnSessionSwitched);
        }

        public ObservableCollection<SessionLoggingRecord> Records { get; set; }

        private void OnSessionSwitched(object sender, SessionSwitchEventArgs e)
        {
            var now = DateTime.Now;

            Records.Add(new SessionLoggingRecord
            {
                DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Day, now.Second),
                Description = e.Reason.ToString()
            });
        }
    }
}
