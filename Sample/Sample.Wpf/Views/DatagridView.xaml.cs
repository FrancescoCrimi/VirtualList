// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CiccioSoft.VirtualList.Sample.Wpf.Views;

public partial class DataGridView : Page
{
    private readonly DataGridViewModel _viewModel;

    public DataGridView()
    {
        InitializeComponent();
        _viewModel = new DataGridViewModel
        {
            ScrollToTop = ScrollToTop,
            UnSelectIndex = UnSelectIndex
        };
        DataContext = _viewModel;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
        => await _viewModel.LoadAsync();

    private void ScrollToTop()
    {
        var scrollViewer = GetVisualChild<ScrollViewer>(dataGrid);
        scrollViewer?.ScrollToTop();
    }

    private void UnSelectIndex()
        => dataGrid.SelectedIndex = -1;

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
