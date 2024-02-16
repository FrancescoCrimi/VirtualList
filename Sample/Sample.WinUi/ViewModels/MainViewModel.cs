// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels
{
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly Frame _frame;

        public MainViewModel(Frame frame)
        {
            _frame = frame;
        }

        [RelayCommand]
        private void OnOpenItemsView()
            => _frame.Navigate(typeof(ItemsViewPage));

        [RelayCommand]
        private void OnOpenListView()
            => _frame.Navigate(typeof(ListViewPage));
    }
}
