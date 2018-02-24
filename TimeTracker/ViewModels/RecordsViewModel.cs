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
            RecordFromSessionLock = new AutoCanExecuteDelegateCommand(
                CreateRecordFromSessionLock,
                CanApplySessionLog);

            SelectedDate = DateTime.Today;
            UpdateRecordList();
        }

        public ICommand UpdateCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand RecordFromSessionLock { get; set; }

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

        public ObservableCollection<SessionLockRecord> SessionLoggerRecords
        {
            get { return sessionLogger.Records; }
        }

        public SessionLockRecord CurrentLoggerRecord
        {
            get { return currentLoggerRecord; }
            set { SetProperty(ref currentLoggerRecord, value); }
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
            {
                var task = Tasks.SingleOrDefault(t => t.Id == record.Task.Id);

                if (task != null)
                    record.Task = task;
            }
                
        }

        private void AddRecordToList()
        {
            AddRecordToList(
                Tasks.First(),
                SelectedDate + DateTime.Now.TimeOfDay,
                SelectedDate + DateTime.Now.TimeOfDay);
        }

        private void AddRecordToList(TaskVM task, DateTime start, DateTime stop)
        {
            if (Tasks == null || Tasks.Count == 0)
                return;

            var record = new RecordVM
            {
                Task = task,
                Start = start,
                Stop = stop
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

        private void CreateRecordFromSessionLock()
        {
            var record = Records.FirstOrDefault(r =>
            r.Start < CurrentLoggerRecord.LockTime &&
            r.Stop > CurrentLoggerRecord.UnlockTime);

            if (record == null)
                return;

            var recordIndex = Records.IndexOf(record);
            var lockTime = (DateTime)CurrentLoggerRecord.LockTime;
            var unlockTime = (DateTime)CurrentLoggerRecord.UnlockTime;

            AddRecordToList(Tasks.First(), lockTime, unlockTime);
            AddRecordToList(record.Task, unlockTime, record.Stop);

            record.Stop = lockTime;
            repository.SaveOrUpdate(record);

            SessionLoggerRecords.Remove(CurrentLoggerRecord);
        }

        private bool CanApplySessionLog()
        {
            return CurrentLoggerRecord != null;
        }

        public ObservableCollection<RecordVM> Records { get; protected set; }
        public ObservableCollection<TaskVM> Tasks { get; protected set; }

        private IProjectDataRepository repository;
        private WindowsSessionLogger sessionLogger;
        private SessionLockRecord currentLoggerRecord;
        private RecordVM currentRecord;
        private DateTime selectedDate;
    }
}
