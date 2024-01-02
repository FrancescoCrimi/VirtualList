using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
        DataContext = mainViewModel;
    }
}
