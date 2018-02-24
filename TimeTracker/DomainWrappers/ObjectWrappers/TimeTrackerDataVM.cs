using System;
using TimeTracker.Domain;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.ObjectWrappers
{
    public class TimeTrackerDataVM :
        VmObjectBase<TimeTrackerData>
    {
        public TimeTrackerDataVM() :
            this(new TimeTrackerData())
        {
        }

        public TimeTrackerDataVM(TimeTrackerData domainObject) :
            base(domainObject)
        {
            if (domainObject == null)
                DomainObject = new TimeTrackerData();
        }

        public Int32 Id
        {
            get { return DomainObject.Id; }
        }

        public double HoursPerDay
        {
            get { return DomainObject.HoursPerDay; }
            set
            {
                DomainObject.HoursPerDay = value;
                RaisePropertyChangedEvent(nameof(HoursPerDay));
            }
        }

        public double OvertimeHours
        {
            get { return DomainObject.OvertimeHours; }
            set
            {
                DomainObject.OvertimeHours = value;
                RaisePropertyChangedEvent(nameof(OvertimeHours));
            }
        }

        public int DaysIntoPast
        {
            get { return DomainObject.DaysIntoPast; }
            set
            {
                DomainObject.DaysIntoPast = value;
                RaisePropertyChangedEvent(nameof(DaysIntoPast));
            }
        }
    }
}
