using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class ItemsViewPage : Page
{
    public ItemsViewViewModel ViewModel { get; }

    public ItemsViewPage()
    {
        InitializeComponent();
        ViewModel = new ItemsViewViewModel();
        DataContext = ViewModel;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
        => await ViewModel.LoadAsync();
}
