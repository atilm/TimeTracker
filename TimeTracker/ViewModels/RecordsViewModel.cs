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
            RecordFromSessionLockCommand = new AutoCanExecuteDelegateCommand(
                CreateRecordFromSessionLock,
                CanApplySessionLog);
            PauseFromSessionLockCommand = new AutoCanExecuteDelegateCommand(
                CreatePauseFromSessionLock,
                CanApplySessionLog);

            SelectedDate = DateTime.Today;
            UpdateRecordList();
        }

        public ICommand UpdateCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand RecordFromSessionLockCommand { get; set; }
        public ICommand PauseFromSessionLockCommand { get; set; }

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
            UpdateTasks();
            AddRecords(repository.GetRecords(SelectedDate));
            ReplaceParentTasksWithObjectsFromTaskList();
        }

        private void AddRecords(IEnumerable<RecordVM> records)
        {
            foreach(var record in records)
            {
                record.PropertyChanged += OnRecordChanged;
            }

            Records.AddRange(records.OrderBy(r => r.Stop));
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
            UpdateTasks();
            if (Tasks == null || Tasks.Count == 0)
                return;

            AddRecordToList(
                Tasks.First(),
                SelectedDate + DateTime.Now.TimeOfDay,
                SelectedDate + DateTime.Now.TimeOfDay);
        }

        private void UpdateTasks()
        {
            Tasks.Clear();
            Tasks.AddRange(repository.GetOpenTasks());
            Tasks.AddRange(repository.GetDoneTasks());
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

            task.AddRecord(record);

            repository.SaveOrUpdate(task);

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
            var record = GetRecordConcernedByLock(CurrentLoggerRecord);

            if (record == null)
                return;

            var recordIndex = Records.IndexOf(record);
            var originalStart = record.Start;
            var originalStop = record.Stop;
            var lockTime = (DateTime)CurrentLoggerRecord.LockTime;
            var unlockTime = (DateTime)CurrentLoggerRecord.UnlockTime;

            if (lockTime == originalStart &&
                unlockTime == originalStop)
            {
                record.Task = Tasks.First();
            }
            else
            {
                AddRecordToList(Tasks.First(), lockTime, unlockTime);

                if (lockTime > originalStart)
                {
                    record.Stop = lockTime;
                }

                if (unlockTime < originalStop)
                {
                    if (record.Stop < unlockTime)
                        AddRecordToList(record.Task, unlockTime, originalStop);
                    else
                        record.Start = unlockTime;
                }
            }

            repository.SaveOrUpdate(record);

            SessionLoggerRecords.Remove(CurrentLoggerRecord);
        }

        private void CreatePauseFromSessionLock()
        {
            var record = GetRecordConcernedByLock(CurrentLoggerRecord);

            if (record == null)
                return;

            var lockTime = (DateTime)CurrentLoggerRecord.LockTime;
            var unlockTime = (DateTime)CurrentLoggerRecord.UnlockTime;

            var recordBeforePause = new RecordVM
            {
                Task = record.Task,
                Start = record.Start,
                Stop = lockTime
            };

            var recordAfterPause = new RecordVM
            {
                Task = record.Task,
                Start = unlockTime,
                Stop = record.Stop
            };

            var task = record.Task;
            var originalRecord = task.Records.Single(r => r.Id == record.Id);
            task.Records.Remove(originalRecord);

            if (recordBeforePause.Duration.TotalMinutes >= 1)
                task.Records.Add(recordBeforePause);

            if (recordAfterPause.Duration.TotalMinutes >= 1)
                task.Records.Add(recordAfterPause);

            repository.SaveOrUpdate(task);
            SessionLoggerRecords.Remove(CurrentLoggerRecord);
            UpdateRecordList();
        }

        private RecordVM GetRecordConcernedByLock(SessionLockRecord lockRecord)
        {
            return Records.FirstOrDefault(r =>
            r.Start <= lockRecord.LockTime &&
            r.Stop >= lockRecord.UnlockTime);
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
