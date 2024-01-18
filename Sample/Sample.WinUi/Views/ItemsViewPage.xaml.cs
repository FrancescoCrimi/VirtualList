using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views
{
    public sealed partial class ItemsViewPage : Page
    {
        public ItemsViewViewModel ViewModel { get; set; }

        public ItemsViewPage()
        {
            InitializeComponent();
            ViewModel = new ItemsViewViewModel();
            DataContext = ViewModel;
        }
    }
}
