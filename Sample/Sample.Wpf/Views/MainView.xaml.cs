// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.ViewModels;
using System.Windows;

namespace CiccioSoft.VirtualList.Sample.Wpf.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        var mainViewModel = new MainViewModel(shellFrame);
        DataContext = mainViewModel;
    }
}
