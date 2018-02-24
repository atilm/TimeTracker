using System.Windows;

namespace TimeTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = App.Container.GetExportedValue<MainWindowViewModel>();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenTasksControl.ViewModel.OnClosing();
        }

        private MainWindowViewModel ViewModel;
    }
}
