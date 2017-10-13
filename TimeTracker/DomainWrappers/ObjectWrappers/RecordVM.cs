using System;
using TimeTracker.Domain;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.ObjectWrappers
{
    public class RecordVM : VmObjectBase<Record>
    {
        public RecordVM() : this(new Record())
        {
        }

        public RecordVM(Record domainObject) : base(domainObject)
        {
            if (domainObject == null)
                DomainObject = new Record();

            Task = new TaskVM(DomainObject.Task);
        }

        public Int32 Id
        {
            get { return DomainObject.Id; }
        }

        public TaskVM Task
        {
            get { return m_task; }

            set
            {
                m_task = value;
                DomainObject.Task = value?.DomainObject;
                RaisePropertyChangedEvent("Task");
            }
        }

        public DateTime Start
        {
            get { return DomainObject.Start; }

            set
            {
                DomainObject.Start = value;
                RaisePropertyChangedEvent("Start");
            }
        }

        public DateTime Stop
        {
            get { return DomainObject.Stop; }

            set
            {
                DomainObject.Stop = value;
                RaisePropertyChangedEvent("Stop");
            }
        }

        public virtual TimeSpan Duration => Stop - Start;
        private TaskVM m_task;
    }
}
