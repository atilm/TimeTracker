using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OpenTasksViewModel : TasksViewModel
    {
        [ImportingConstructor]
        public OpenTasksViewModel(
            IProjectDataRepository repository,
            TaskTimer taskTimer)
            : base(repository)
        {
            TaskTimer = taskTimer;
            ActiveTask = null;
            AttachEvents();
        }

        private void AttachEvents()
        {
            TaskTimer.PropertyChanged += OnTimerPropertyChanged;
        }

        private void OnTimerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskTimer.IsMeasuring))
                HandleTimerToggled();
        }

        private void HandleTimerToggled()
        {
            if (!TaskTimer.IsMeasuring)
                AddRecord();
        }

        private void AddRecord()
        {
            var record = new RecordVM
            {
                Task = ActiveTask,
                Start = GetRoundedDateTime(TaskTimer.StartTime),
                Stop = GetRoundedDateTime(DateTime.Now)
            };

            repository.SaveOrUpdate(record);
            TaskTimer.Reset();
        }

        private DateTime GetRoundedDateTime(DateTime t)
        {
            return new DateTime(
                t.Year, t.Month, t.Day,
                t.Hour, t.Minute, 0, t.Kind);
        }

        protected override ObservableCollection<TaskVM> GetTasksFromRepository()
        {
            return repository.GetOpenTasks();
        }

        protected override void LoadTasks()
        {
            base.LoadTasks();

            if (Tasks.Count == 0)
                return;
            
            ActiveTask = Tasks.First();
        }

        public TaskVM ActiveTask
        {
            get { return activeTask; }
            set
            {
                SetProperty(ref activeTask, value);
                RaisePropertyChanged(nameof(IsActiveTaskSelected));
            }
        }

        public bool IsActiveTaskSelected
        {
            get { return ActiveTask != null; }
        }

        public void SwitchActiveTask(TaskVM task)
        {
            if (TaskTimer.IsMeasuring)
            {
                AddRecord();
            }

            ActiveTask = task;
        }

        public TaskTimer TaskTimer { get; protected set; }

        private TaskVM activeTask;
    }
}
