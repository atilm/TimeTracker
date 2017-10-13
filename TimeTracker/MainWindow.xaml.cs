using System.Windows;

namespace TimeTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Container.GetExportedValue<MainWindowViewModel>();
        }
    }
}
