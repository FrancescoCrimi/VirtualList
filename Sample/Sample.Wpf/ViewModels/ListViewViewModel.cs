// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

public partial class ListViewViewModel : ObservableObject
{
    public ListViewViewModel()
        => Items = new ModelVirtualCollection();

    public ModelVirtualCollection Items { get; }
    public Action? ScrollToTop { set => Items.ScrollToTop = value; }
    public Action? UnSelectIndex { set => Items.UnSelectIndex = value; }

    public Task LoadAsync()
        => Items.LoadAsync("");

    [RelayCommand]
    private Task OnSearch(string searchString)
        => Items.LoadAsync(searchString);
}
