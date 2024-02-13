using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.Uwp
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
        }

        private async void OnPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
            => await ViewModel.LoadAsync();
    }
}
