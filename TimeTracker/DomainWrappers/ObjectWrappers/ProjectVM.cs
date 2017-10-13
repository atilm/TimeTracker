using System;
using TimeTracker.Domain;
using TimeTracker.DomainWrappers.CollectionWrappers;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.ObjectWrappers
{
    public class ProjectVM : VmObjectBase<Project>
    {
        public ProjectVM() : this(new Project())
        {
        }

        public ProjectVM(Project domainObject) : base(domainObject)
        {
            if (domainObject == null)
                DomainObject = new Project();
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

        public string ProjectNumber
        {
            get { return DomainObject.ProjectNumber; }

            set
            {
                DomainObject.ProjectNumber = value;
                RaisePropertyChangedEvent("ProjectNumber");
            }
        }

        public bool IsActive
        {
            get { return DomainObject.IsActive; }

            set
            {
                DomainObject.IsActive = value;
                RaisePropertyChangedEvent("IsActive");
            }
        }

        public TasksVM Tasks
        {
            get
            {
                if (m_tasks == null)
                {
                    m_tasks = new TasksVM(DomainObject.Tasks, this);
                }

                return m_tasks;
            }
            set
            {
                m_tasks = value;
            }
        }

        public void AddTask(TaskVM task)
        {
            task.Project = this;
            Tasks.Add(task);
        }

        private TasksVM m_tasks;
    }
}
