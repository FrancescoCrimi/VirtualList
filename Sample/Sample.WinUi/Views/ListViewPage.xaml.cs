// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class ListViewPage : Page
{
    public ListViewViewModel ViewModel { get; }

    public ListViewPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<ListViewViewModel>();
        DataContext = ViewModel;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
        => await ViewModel.LoadAsync();
}
