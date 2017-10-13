using System.Windows.Controls;
using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    public partial class RecordsView : UserControl
    {
        public RecordsView()
        {
            InitializeComponent();
            DataContext = App.Container.GetExportedValue<RecordsViewModel>();
        }
    }
}
