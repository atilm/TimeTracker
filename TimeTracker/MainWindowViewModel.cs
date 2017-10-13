using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.DbMappings;

namespace TimeTracker
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainWindowViewModel
    { 
        [ImportingConstructor]
        public MainWindowViewModel(IProjectDataRepository repository)
        {
            m_repository = repository;
        }

        private IProjectDataRepository m_repository;
    }
}
