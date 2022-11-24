using System;
using System.Windows;
using CiccioSoft.VirtualList.Wpf.ViewModels;

namespace CiccioSoft.VirtualList.Wpf.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel(shellFrame);
            DataContext = mainViewModel;
        }
    }
}
