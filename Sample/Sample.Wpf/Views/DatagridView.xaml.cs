using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

namespace CiccioSoft.VirtualList.Sample.Wpf.Views
{
    public partial class DatagridView : Page
    {
        private readonly DatagridViewModel viewModel;

        public DatagridView()
        {
            InitializeComponent();
            viewModel = new DatagridViewModel();
            DataContext = viewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetVisualChild<ScrollViewer>(dataGrid);
            scrollViewer?.ScrollToTop();
            await viewModel.SearchAsync();
        }

        private T? GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
