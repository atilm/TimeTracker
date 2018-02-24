using TimeTracker.Domain;
using FluentNHibernate.Mapping;

namespace TimeTracker.DbMappings
{
    public class TaskMapping : ClassMap<Task>
    {
        public TaskMapping()
        {
            Table("tasks");

            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsDone);
            Map(x => x.DoneDate);
            References(x => x.Project);
            HasMany(x => x.Records)
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
