using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimeTracker.DomainWrappers.ObjectWrappers;
using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    public partial class TasksView : UserControl
    {
        public TasksView()
        {
            InitializeComponent();
            DataContext = ViewModel = App.Container.GetExportedValue<OpenTasksViewModel>();
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)e.OriginalSource).DataContext is TaskVM doubleClickedTask)
            {
                ViewModel.SwitchActiveTask(doubleClickedTask);
            }
        }

        public OpenTasksViewModel ViewModel;
    }
}
