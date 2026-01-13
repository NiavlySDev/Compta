using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
