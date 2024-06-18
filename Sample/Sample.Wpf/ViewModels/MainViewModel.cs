// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly Frame _frame;

    public MainViewModel(Frame frame)
        => _frame = frame;

    [RelayCommand]
    public void OnOpenDataGrid()
        => _frame.Navigate(new DataGridView());

    [RelayCommand]
    public void OnOpenListView()
        => _frame.Navigate(new ListViewView());
}
