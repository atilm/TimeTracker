using System.Windows.Controls;
using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = ViewModels =
                App.Container.GetExportedValue<SettingsViewModel>();
        }

        public SettingsViewModel ViewModels { get; }
    }
}
