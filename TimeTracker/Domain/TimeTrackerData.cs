
using System;

namespace TimeTracker.Domain
{
    public class TimeTrackerData
    {
        public virtual Int32 Id { get; protected set; }
        public virtual double HoursPerDay { get; set; }
        public virtual double OvertimeHours { get; set; }
    }
}
