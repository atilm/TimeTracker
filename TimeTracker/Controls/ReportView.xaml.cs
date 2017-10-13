using System.Windows.Controls;
using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
            DataContext = App.Container.GetExportedValue<ReportViewModel>();
        }
    }
}
