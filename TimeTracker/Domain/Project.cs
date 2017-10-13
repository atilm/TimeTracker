using System;
using System.Collections.Generic;

namespace TimeTracker.Domain
{
    public class Project
    {
        public Project()
        {
            Tasks = new List<Task>();

            Name = "New Project";
            ProjectNumber = "Number";
            IsActive = true;
        }

        public virtual void AddTask(Task task)
        {
            task.Project = this;
            Tasks.Add(task);
        }

        public virtual Int32 Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string ProjectNumber { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual IList<Task> Tasks { get; set; }
    }
}
