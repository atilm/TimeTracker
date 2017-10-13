using Prism.Mvvm;
using System.ComponentModel.Composition;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsViewModel : BindableBase
    {
        [ImportingConstructor]
        public SettingsViewModel(IProjectDataRepository repository)
        {
            this.repository = repository;
            appData = repository.GetAppData();
        }

        public double HoursPerDay
        {
            get { return appData.HoursPerDay; }
            set
            {
                appData.HoursPerDay = value;
                repository.SaveOrUpdate(appData);
            }
        }

        private TimeTrackerDataVM appData;
        private IProjectDataRepository repository;
    }
}
