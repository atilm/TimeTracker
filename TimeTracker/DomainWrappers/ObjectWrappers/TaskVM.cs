using System;
using System.Linq;
using TimeTracker.Domain;
using TimeTracker.DomainWrappers.CollectionWrappers;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.ObjectWrappers
{
    public class TaskVM : VmObjectBase<Task>
    {
        public TaskVM() : this(new Task())
        {
        }

        public TaskVM(Task domainObject) : base(domainObject)
        {
            if (domainObject == null)
                DomainObject = new Task();

            Project = new ProjectVM(DomainObject.Project);
        }

        public Int32 Id
        {
            get { return DomainObject.Id; }
        }

        public string Name
        {
            get { return DomainObject.Name; }

            set
            {
                DomainObject.Name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

        public bool IsDone
        {
            get { return DomainObject.IsDone; }

            set
            {
                DomainObject.IsDone = value;
                RaisePropertyChangedEvent("IsDone");
            }
        }

        public DateTime DoneDate
        {
            get { return DomainObject.DoneDate; }

            set
            {
                DomainObject.DoneDate = value;
                RaisePropertyChangedEvent("DoneDate");
            }
        }

        public ProjectVM Project
        {
            get { return m_project; }

            set
            {
                m_project = value;
                DomainObject.Project = value?.DomainObject;
                RaisePropertyChangedEvent("Project");
            }
        }

        public RecordsVM Records
        {
            get
            {
                if (m_records == null)
                {
                    m_records = new RecordsVM(DomainObject.Records, this);
                }

                return m_records;
            }
            set
            {
                m_records = value;
            }
        }

        public void AddRecord(RecordVM record)
        {
            record.Task = this;
            Records.Add(record);
        }

        public void RemoveRecord(RecordVM record)
        {
            var originalRecord = Records.FirstOrDefault(r => r.Id == record.Id);
            if (originalRecord != null)
                Records.Remove(originalRecord);
        }

        public void SwitchProject(ProjectVM newProject)
        {
            if (Project != null)
                Project.Tasks.Remove(this);

            Project = newProject;
            Project.Tasks.Add(this);
        }

        private ProjectVM m_project;
        private RecordsVM m_records;
    }
}
