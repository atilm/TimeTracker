using TimeTracker.Domain;
using FluentNHibernate.Mapping;

namespace TimeTracker.DbMappings
{
    public class ProjectMapping : ClassMap<Project>
    {
        public ProjectMapping()
        {
            Table("projects");

            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ProjectNumber);
            Map(x => x.IsActive);

            HasMany(x => x.Tasks)
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
