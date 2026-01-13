using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views
{
    public partial class EmployeesView : UserControl
    {
        public EmployeesView(EmployeesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
