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
            var doubleClickedTask = ((FrameworkElement)e.OriginalSource).DataContext as TaskVM;

            if(doubleClickedTask != null)
            {
                ViewModel.SwitchActiveTask(doubleClickedTask);
            }
        }

        private OpenTasksViewModel ViewModel;
    }
}
