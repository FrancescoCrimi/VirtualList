// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using WinUIEx;

namespace CiccioSoft.VirtualList.Sample.WinUi.Views;

public sealed partial class MainWindow : WindowEx
{
    public MainViewModel ViewModel { get; }
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel(shellFrame);
    }
}
