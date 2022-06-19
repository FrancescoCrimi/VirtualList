using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp;

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
