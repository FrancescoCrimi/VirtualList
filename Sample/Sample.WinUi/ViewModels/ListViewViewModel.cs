// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Collection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.ViewModels;

public partial class ListViewViewModel : ObservableRecipient
{
    public ListViewViewModel()
        => Items = new ModelVirtualCollection();

    public ModelVirtualCollection Items { get; }

    internal Task LoadAsync()
        => Items.LoadAsync("");

    [RelayCommand]
    private Task OnSearch(string searchString)
        => Items.LoadAsync(searchString);
}
