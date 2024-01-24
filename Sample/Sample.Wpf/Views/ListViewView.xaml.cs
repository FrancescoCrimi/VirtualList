using CiccioSoft.VirtualList.Sample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CiccioSoft.VirtualList.Sample.Wpf.Views;

public partial class ListViewView : Page
{
    private readonly ListViewViewModel viewModel;

    public ListViewView()
    {
        InitializeComponent();
        viewModel = new ListViewViewModel
        {
            ScrollToTop = ScrollToTop,
            UnSelectIndex = UnSelectIndex
        };
        DataContext = viewModel;
    }

    private void ScrollToTop()
    {
        listView.SelectedIndex = -1;
        var scrollViewer = GetVisualChild<ScrollViewer>(listView);
        scrollViewer?.ScrollToTop();
    }

    private void UnSelectIndex()
        => listView.SelectedIndex = -1;

    /// <summary>
    /// Cerca a ritroso un particolare oggetto grafico all'interno
    /// di un oggetto dato come parametro
    /// </summary>
    /// <typeparam name="T">Tipo da cercare</typeparam>
    /// <param name="parent">oggetto in cui cercare</param>
    /// <returns>oggetto trovato o null</returns>
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
