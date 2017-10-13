namespace TimeTracker.WrappingFramework
{
    public abstract class VmObjectBase<DM> : ViewModelBase
    {
        protected VmObjectBase(DM domainObject)
        {
            DomainObject = domainObject;
        }

        internal DM DomainObject { get; set; }
    }
}
