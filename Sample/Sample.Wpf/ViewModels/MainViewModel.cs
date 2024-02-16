// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly Frame _frame;

    public MainViewModel(Frame frame)
    {
        _frame = frame;
    }

    [RelayCommand]
    public async Task OnOpenDataGrid()
        => await Application.Current.Dispatcher.InvokeAsync(()
            => _frame.Navigate(new DatagridView()));

    [RelayCommand]
    public async Task OnOpenListView()
         => await Application.Current.Dispatcher.InvokeAsync(()
            => _frame.Navigate(new ListViewView()));
}