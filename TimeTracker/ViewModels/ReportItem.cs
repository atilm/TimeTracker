using Prism.Mvvm;
using System;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    public class ReportItem : BindableBase
    {
        private double scaledDurationInHours;

        public virtual string Title { get; set; }
        public virtual TimeSpan Duration { get; set; }

        public virtual bool IsResult { get; set; }

        public virtual double DurationInHours
        {
            get { return Math.Round(Duration.TotalHours, 2); }
        }

        public virtual double ScaledDurationInHours
        {
            get { return scaledDurationInHours; }
            set { SetProperty(ref scaledDurationInHours, Math.Round(value, 2));  }
        }

        public virtual bool BelongsToProject(ProjectVM project)
        {
            return false;
        }
    }
}
