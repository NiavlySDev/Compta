using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views;

public partial class TransactionsView : UserControl
{
    public TransactionsView(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
