using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TimeTracker.DomainWrappers.ObjectWrappers;
using TimeTracker.ViewModels;

namespace TimeTracker.DbMappings
{
    public interface IProjectDataRepository
    {
        event EventHandler RaiseProjectsChangedEvent;
        event EventHandler RaiseTasksChangedEvent;
        event EventHandler RaiseRecordsChangedEvent;

        void SaveOrUpdate(TimeTrackerDataVM appData);
        TimeTrackerDataVM GetAppData();

        void SaveOrUpdate(ProjectVM project);
        void Delete(ProjectVM project);
        ObservableCollection<ProjectVM> GetProjects();
        ObservableCollection<ProjectVM> GetActiveProjects();

        void SaveOrUpdate(TaskVM task);
        void Delete(TaskVM task);
        ObservableCollection<TaskVM> GetOpenTasks();
        ObservableCollection<TaskVM> GetDoneTasks();

        ProjectVM GetProjectByNumber(string number);

        void SaveOrUpdate(RecordVM record);
        void SaveOrUpdate(ICollection<RecordVM> records);
        void Delete(RecordVM record);
        ObservableCollection<RecordVM> GetRecords(DateTime date);

        void CloseSession();
    }
}
