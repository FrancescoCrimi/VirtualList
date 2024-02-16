// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class ItemsViewPage : Page
{
    public ItemsViewViewModel ViewModel { get; }

    public ItemsViewPage()
    {
        InitializeComponent();
        ViewModel = new ItemsViewViewModel();
        DataContext = ViewModel;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
        => await ViewModel.LoadAsync();
}
