using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    public class ProjectReportItem : ReportItem
    {
        public ProjectReportItem(ProjectVM project)
        {
            Project = project;
        }

        public ProjectVM Project { get; set; }

        public override string Title
        {
            get { return $"{Project.Name} ({Project.ProjectNumber})"; }
            set { }
        }

        public override bool BelongsToProject(ProjectVM project)
        {
            return project.Id == Project.Id;
        }
    }
}
