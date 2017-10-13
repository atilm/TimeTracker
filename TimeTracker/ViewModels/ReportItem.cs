using Prism.Mvvm;
using System;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    public class ReportItem : BindableBase
    {
        public ProjectVM Project { get; set; }
        public TimeSpan Duration { get; set; }

        public double DurationInHours
        {
            get { return Math.Round(Duration.TotalHours, 2); }
        }

        public bool BelongsToProject(ProjectVM project)
        {
            return project.Id == Project.Id;
        }
    }
}
