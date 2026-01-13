using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views;

public partial class SuppliersView : UserControl
{
    public SuppliersView(SuppliersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}