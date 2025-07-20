// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CiccioSoft.VirtualList.Sample.Uwp
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e) =>
            await ViewModel.LoadAsync();

        private async void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var searchString = SearchTextBox.Text ?? string.Empty;
            await ViewModel.LoadAsync(searchString);
        }
    }
}
