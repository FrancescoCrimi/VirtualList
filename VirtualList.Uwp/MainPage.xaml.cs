using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Uwp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            DataContext = mainViewModel;
        }
    }
}
