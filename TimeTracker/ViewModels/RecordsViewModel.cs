using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using TimeTracker.Commands;
using TimeTracker.DbMappings;
using TimeTracker.DomainWrappers.ObjectWrappers;
using TimeTracker.TimeTracking;

namespace TimeTracker.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecordsViewModel : BindableBase
    {
        [ImportingConstructor]
        public RecordsViewModel(
            IProjectDataRepository repository,
            WindowsSessionLogger sessionLogger)
        {
            this.repository = repository;
            this.repository.RaiseRecordsChangedEvent += OnRepositoryChanged;

            this.sessionLogger = sessionLogger;

            Records = new ObservableCollection<RecordVM>();
            Tasks = new ObservableCollection<TaskVM>();

            UpdateCommand = new DelegateCommand(UpdateRecordList);
            AddCommand = new DelegateCommand(AddRecordToList);
            DeleteCommand = new AutoCanExecuteDelegateCommand(DeleteRecord, CanDeleteRecord);
            SaveAllCommand = new AutoCanExecuteDelegateCommand(SaveAll, CanSave);

            SelectedDate = DateTime.Today;
            UpdateRecordList();
        }

        public ICommand UpdateCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }

        public RecordVM CurrentRecord
        {
            get { return currentRecord; }
            set { SetProperty(ref currentRecord, value); }
        }

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { SetProperty(ref selectedDate, value); }
        }

        public ObservableCollection<SessionLoggingRecord> SessionLoggerRecords
        {
            get { return sessionLogger.Records; }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedDate))
                UpdateRecordList();
        }

        private void OnRepositoryChanged(object sender, EventArgs e)
        {
            UpdateRecordList();
        }

        private void UpdateRecordList()
        {
            ClearRecordList();
            Tasks.Clear();

            Tasks.AddRange(repository.GetOpenTasks());
            Tasks.AddRange(repository.GetDoneTasks());
            AddRecords(repository.GetRecords(SelectedDate));
            ReplaceParentTasksWithObjectsFromTaskList();
        }

        private void AddRecords(IEnumerable<RecordVM> records)
        {
            foreach(var record in records)
            {
                record.PropertyChanged += OnRecordChanged;
            }

            Records.AddRange(records);
        }

        private void OnRecordChanged(object sender, PropertyChangedEventArgs e)
        {
            repository.RaiseRecordsChangedEvent -= OnRepositoryChanged;
            repository.SaveOrUpdate(sender as RecordVM);
            repository.RaiseRecordsChangedEvent += OnRepositoryChanged;
        }

        private void ClearRecordList()
        {
            foreach(var record in Records)
            {
                record.PropertyChanged -= OnRecordChanged;
            }

            Records.Clear();
        }

        private void ReplaceParentTasksWithObjectsFromTaskList()
        {
            foreach (var record in Records)
                record.Task = Tasks.Single(t => t.Id == record.Task.Id);
        }

        private void AddRecordToList()
        {
            if (Tasks == null || Tasks.Count == 0)
                return;

            var record = new RecordVM
            {
                Task = Tasks.First(),
                Start = SelectedDate + DateTime.Now.TimeOfDay,
                Stop = SelectedDate + DateTime.Now.TimeOfDay
            };

            repository.SaveOrUpdate(record);

            UpdateRecordList();
        }

        private bool CanDeleteRecord()
        {
            return CurrentRecord != null;
        }

        private void DeleteRecord()
        {
            repository.Delete(CurrentRecord);
            CurrentRecord = null;
            UpdateRecordList();
        }

        private bool CanSave()
        {
            return true;
        }

        private void SaveAll()
        {
            repository.SaveOrUpdate(Records);
        }
        
        public ObservableCollection<RecordVM> Records { get; protected set; }
        public ObservableCollection<TaskVM> Tasks { get; protected set; }

        private IProjectDataRepository repository;
        private WindowsSessionLogger sessionLogger;
        private RecordVM currentRecord;
        private DateTime selectedDate;
    }
}
