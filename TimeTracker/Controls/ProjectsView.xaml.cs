using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeTracker.ViewModels;

namespace TimeTracker.Controls
{
    /// <summary>
    /// Interaktionslogik für ProjectsView.xaml
    /// </summary>
    public partial class ProjectsView : UserControl
    {
        public ProjectsView()
        {
            InitializeComponent();
            DataContext = App.Container.GetExportedValue<ProjectsViewModel>();
        }
    }
}
