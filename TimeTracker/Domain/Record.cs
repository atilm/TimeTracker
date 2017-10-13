using System;

namespace TimeTracker.Domain
{
    public class Record
    {
        public virtual Int32 Id { get; protected set; }
        public virtual Task Task { get; set; }
        public virtual DateTime Start { get; set; }
        public virtual DateTime Stop { get; set; } 
    }
}
