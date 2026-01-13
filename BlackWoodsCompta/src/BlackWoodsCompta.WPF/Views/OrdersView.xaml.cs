using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views
{
    public partial class OrdersView : System.Windows.Controls.UserControl
    {
        public OrdersView(OrdersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}