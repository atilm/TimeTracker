using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTracker.Domain;
using TimeTracker.DomainWrappers.ObjectWrappers;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.CollectionWrappers
{
    public class TasksVM : VmCollectionBase<TaskVM, Task>
    {
        public TasksVM(IList<Task> domainCollection, ProjectVM parent)
            : base(domainCollection)
        {
            foreach(var task in this)
            {
                task.Project = parent;
            }
        }
    }
}
