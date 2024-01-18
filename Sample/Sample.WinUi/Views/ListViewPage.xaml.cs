using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class ListViewPage : Page
{
    public ListViewPage()
    {
        InitializeComponent();
        var mainViewModel = Ioc.Default.GetRequiredService<ListViewViewModel>();
        DataContext = mainViewModel;
    }
}
