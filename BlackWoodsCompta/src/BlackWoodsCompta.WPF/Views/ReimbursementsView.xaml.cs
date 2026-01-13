using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views;

public partial class ReimbursementsView : UserControl
{
    public ReimbursementsView(ReimbursementsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}