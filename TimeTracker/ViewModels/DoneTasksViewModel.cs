using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DoneTasksViewModel : TasksViewModel
    {
        [ImportingConstructor]
        public DoneTasksViewModel(IProjectDataRepository repository)
            : base(repository)
        {
            repository.RaiseAppDataChangedEvent += OnTasksChanged;
        }

        protected override ObservableCollection<TaskVM> GetTasksFromRepository()
        {
            return repository.GetDoneTasks();
        }
    }
}
