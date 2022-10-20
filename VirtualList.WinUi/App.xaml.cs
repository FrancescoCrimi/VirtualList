using CiccioSoft.VirtualList.Data.Infrastructure;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VirtualList.WinUi.ViewModels;
using VirtualList.WinUi.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VirtualList.WinUi
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window m_window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            var host = Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                //// Default Activation Handler
                //services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                //// Other Activation Handlers


                // aggiungi data
                services.AddData(context.Configuration);


                //// Services
                //services.AddSingleton<IActivationService, ActivationService>();
                //services.AddSingleton<IPageService, PageService>();
                //services.AddSingleton<INavigationService, NavigationService>();

                //// Core Services
                //services.AddSingleton<ISampleDataService, SampleDataService>();
                //services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainPage>();

                // Configuration
                
            }).
            Build();
            Ioc.Default.ConfigureServices(host.Services);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Content = new MainPage();
            m_window.Activate();
        }
    }
}
