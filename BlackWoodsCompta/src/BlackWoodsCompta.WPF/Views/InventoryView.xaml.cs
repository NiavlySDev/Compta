using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views
{
    public partial class InventoryView : UserControl
    {
        public InventoryView(InventoryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
