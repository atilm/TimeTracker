using System;
using System.Collections.Generic;

namespace TimeTracker.Domain
{
    public class Task
    {
        public Task()
        {
            Records = new List<Record>();
        }

        public virtual void AddRecord(Record record)
        {
            record.Task = this;
            Records.Add(record);
        }

        public virtual Int32 Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual bool IsDone { get; set; }
        public virtual DateTime DoneDate { get; set; }
        public virtual Project Project { get; set; }
        public virtual IList<Record> Records { get; set; }
    }
}
