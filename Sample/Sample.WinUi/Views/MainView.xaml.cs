using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using Microsoft.UI.Xaml;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class MainView : Window
{
    public MainViewModel ViewModel { get; }
    public MainView()
    {
        InitializeComponent();
        ViewModel = new MainViewModel(shellFrame);
    }
}
