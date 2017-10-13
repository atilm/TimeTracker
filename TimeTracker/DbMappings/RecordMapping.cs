using TimeTracker.Domain;
using FluentNHibernate.Mapping;

namespace TimeTracker.DbMappings
{
    public class RecordMapping : ClassMap<Record>
    {
        public RecordMapping()
        {
            Table("records");

            Id(x => x.Id);
            Map(x => x.Start);
            Map(x => x.Stop);
            References(x => x.Task);
        }
    }
}
