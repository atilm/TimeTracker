using System.Linq;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Linq;
using TimeTracker.Domain;
using TimeTracker.DomainWrappers.ObjectWrappers;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.IO;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace TimeTracker.DbMappings
{
    [Export(typeof(IProjectDataRepository))]
    public class ProjectDataRepository : IProjectDataRepository
    {
        public ProjectDataRepository() : this(false)
        {
        }

        public ProjectDataRepository(bool alwaysCreateNew)
        {
            this.alwaysCreateNew = alwaysCreateNew;

            sessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(dbFilePath))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProjectMapping>())
                .ExposeConfiguration(UpdateSchema)
                .BuildSessionFactory();
        }

        public event EventHandler RaiseProjectsChangedEvent;
        public event EventHandler RaiseTasksChangedEvent;
        public event EventHandler RaiseRecordsChangedEvent;
        public event EventHandler RaiseAppDataChangedEvent;

        public void SaveOrUpdate(TimeTrackerDataVM appData)
        {
            AssureSession();

            using( var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(appData.DomainObject);
                transaction.Commit();
                OnRaiseAppDataChangedEvent();
            }
        }

        public TimeTrackerDataVM GetAppData()
        {
            AssureSession();

            var appData = session.Query<TimeTrackerData>().SingleOrDefault();
            return new TimeTrackerDataVM(appData);
        }

        public void SaveOrUpdate(ProjectVM project)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(project.DomainObject);
                transaction.Commit();
                OnRaiseProjectsChangedEvent();
            }
        }

        public void Delete(ProjectVM project)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.Delete(project.DomainObject);
                transaction.Commit();
                OnRaiseProjectsChangedEvent();
            }
        }

        public ObservableCollection<ProjectVM> GetProjects()
        {
            AssureSession();

            var domainProjects = session.Query<Project>().ToList();
            return CreateObservableCollection(domainProjects);
        }

        public ObservableCollection<ProjectVM> GetActiveProjects()
        {
            AssureSession();

            var projects = session.Query<Project>().Where(p => p.IsActive).ToList();
            return CreateObservableCollection(projects);
        }

        public void SaveOrUpdate(TaskVM task)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(task.DomainObject);
                transaction.Commit();
                OnRaiseTasksChangedEvent();
            }
        }

        public void Delete(TaskVM task)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.Delete(task.DomainObject);
                transaction.Commit();
                OnRaiseTasksChangedEvent();
            }
        }

        public ObservableCollection<TaskVM> GetOpenTasks()
        {
            AssureSession();

            var tasks = session.Query<Task>()
                .Where(t => t.IsDone == false)
                .ToList();

            return CreateObservableCollection(tasks);
        }

        public ObservableCollection<TaskVM> GetDoneTasks()
        {
            AssureSession();

            var settings = GetAppData();
            var earliestDate = DateTime.Now.Date - TimeSpan.FromDays(settings.DaysIntoPast);
            var invalidDate = new DateTime(1900, 01, 01);

            var tasks = session.Query<Task>()
                .Where(t => t.IsDone &&
                       (t.DoneDate >= earliestDate ||
                        t.DoneDate < invalidDate ))
                .OrderByDescending(t => t.DoneDate)
                .ToList();

            return CreateObservableCollection(tasks);
        }

        public ProjectVM GetProjectByNumber(string number)
        {
            AssureSession();

            var domainObject = session.Query<Project>()
                     .Where(p => p.ProjectNumber == number)
                     .FirstOrDefault();

            return new ProjectVM(domainObject);
        }

        public void SaveOrUpdate(RecordVM record)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(record.DomainObject);
                transaction.Commit();
                OnRaiseRecordsChangedEvent();
            }
        }

        public void SaveOrUpdate(ICollection<RecordVM> records)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                foreach(var record in records)
                    session.SaveOrUpdate(record.DomainObject);

                transaction.Commit();
                OnRaiseRecordsChangedEvent();
            }
        }

        public void Delete(RecordVM record)
        {
            AssureSession();

            using (var transaction = session.BeginTransaction())
            {
                session.Delete(record.DomainObject);
                transaction.Commit();
                OnRaiseRecordsChangedEvent();
            }
        }

        public ObservableCollection<RecordVM> GetRecords(DateTime date)
        {
            AssureSession();

            var records = session.Query<Record>()
                .Where(r => r.Stop.Date == date.Date)
                .Fetch(r => r.Task)
                .ToList();

            return CreateObservableCollection(records);
        }

        public void CloseSession()
        {
            session.Close();
            session = null;
        }

        private ObservableCollection<ProjectVM> CreateObservableCollection(
            IList<Project> domainProjects)
        {
            var projectVMs = domainProjects
                .Select(x => new ProjectVM(x));

            return new ObservableCollection<ProjectVM>(projectVMs);
        }

        private ObservableCollection<TaskVM> CreateObservableCollection(
            IList<Task> domainTasks)
        {
            var taskVMs = domainTasks
                .Select(x => new TaskVM(x));

            return new ObservableCollection<TaskVM>(taskVMs);
        }

        private ObservableCollection<RecordVM> CreateObservableCollection(
            IList<Record> domainRecord)
        {
            var recordVMs = domainRecord
                .Select(x => new RecordVM(x));

            return new ObservableCollection<RecordVM>(recordVMs);
        }

        private void AssureSession()
        {
            if (session != null && session.IsOpen)
                return;

            session = sessionFactory.OpenSession();
        }

        private void UpdateSchema(Configuration configuration)
        {
            if (alwaysCreateNew)
            {
                if (File.Exists(dbFilePath))
                    File.Delete(dbFilePath);
            }

            var schemaUpdate = new SchemaUpdate(configuration);
            schemaUpdate.Execute(false, true);
        }

        protected virtual void OnRaiseProjectsChangedEvent()
        {
            RaiseProjectsChangedEvent?.Invoke(this, null);
        }

        protected virtual void OnRaiseTasksChangedEvent()
        {
            RaiseTasksChangedEvent?.Invoke(this, null);
        }

        protected virtual void OnRaiseRecordsChangedEvent()
        {
            RaiseRecordsChangedEvent?.Invoke(this, null);
        }

        protected virtual void OnRaiseAppDataChangedEvent()
        {
            RaiseAppDataChangedEvent?.Invoke(this, null);
        }

        private ISession session;
        private ISessionFactory sessionFactory;
        private string dbFilePath = "TimeTracker.db";
        private bool alwaysCreateNew;
    }
}
