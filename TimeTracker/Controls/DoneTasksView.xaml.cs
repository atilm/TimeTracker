using System.Windows.Controls;

using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    public partial class DoneTasksView : UserControl
    {
        public DoneTasksView()
        {
            InitializeComponent();
            DataContext = App.Container.GetExportedValue<DoneTasksViewModel>();
        }
    }
}
