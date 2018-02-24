using FluentNHibernate.Mapping;
using TimeTracker.Domain;

namespace TimeTracker.DbMappings
{
    public class TimeTrackerDataMapping :
        ClassMap<TimeTrackerData>
    {
        public TimeTrackerDataMapping()
        {
            Table("time_tracker_data");
            Id(x => x.Id);
            Map(x => x.HoursPerDay);
            Map(x => x.OvertimeHours);
            Map(x => x.DaysIntoPast);
        }
    }
}
