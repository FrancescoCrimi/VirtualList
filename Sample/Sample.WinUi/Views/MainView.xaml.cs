// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using Microsoft.UI.Xaml;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class MainView : Window
{
    public MainViewModel ViewModel { get; }
    public MainView()
    {
        InitializeComponent();
        ViewModel = new MainViewModel(shellFrame);
    }
}
