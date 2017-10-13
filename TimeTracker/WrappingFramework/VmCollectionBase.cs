using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TimeTracker.WrappingFramework
{
    public abstract class VmCollectionBase<VM, DM> : ObservableCollection<VM>
    {
        public VmCollectionBase(IList<DM> domainCollection)
        {
            m_DomainCollection = domainCollection;

            m_EventsDisabled = true;

            /* Note that we can't simply new-up a VM object, because new() will
             * not let us use a paramaterized constructor on a generic type. See
             * http://msdn.microsoft.com/en-us/library/tass7xkw.aspx. So, we use 
             * Activator.CreateInstance() instead. */

            // Populate this collection with VM objects
            foreach (var domainObject in domainCollection)
            {
                var paramList = new object[] { domainObject };
                var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
                this.Add(wrapperObject);
            }

            m_EventsDisabled = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (m_EventsDisabled) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddDomainObjects(e);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveDomainObjects(e);
                    break;
            }
        }

        private void AddDomainObjects(NotifyCollectionChangedEventArgs e)
        {
            foreach (VmObjectBase<DM> wrapperObject in e.NewItems)
            {
                var domainObject = wrapperObject.DomainObject;
                m_DomainCollection.Add(domainObject);
            }
        }

        private void RemoveDomainObjects(NotifyCollectionChangedEventArgs e)
        {
            foreach (VmObjectBase<DM> wrapperObject in e.OldItems)
            {
                var domainObject = wrapperObject.DomainObject;
                m_DomainCollection.Remove(domainObject);
            }
        }

        private IList<DM> m_DomainCollection;
        private bool m_EventsDisabled;
    }
}
