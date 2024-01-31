using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadAsync();
        }
    }
}
