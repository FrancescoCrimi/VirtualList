using System.Windows;

namespace CiccioSoft.VirtualList.Wpf
{
    public partial class MainView : Window
    {
        public MainView(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}
