using System.Collections.Generic;
using TimeTracker.Domain;
using TimeTracker.DomainWrappers.ObjectWrappers;
using TimeTracker.WrappingFramework;

namespace TimeTracker.DomainWrappers.CollectionWrappers
{
    public class RecordsVM : VmCollectionBase<RecordVM, Record>
    {
        public RecordsVM(IList<Record> domainCollection, TaskVM parent)
            : base(domainCollection)
        {
            foreach (var record in this)
            {
                record.Task = parent;
            }
        }
    }
}
