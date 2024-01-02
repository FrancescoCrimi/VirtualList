using System;
using System.Windows;
using CiccioSoft.VirtualList.Sample.Wpf.ViewModels;

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
