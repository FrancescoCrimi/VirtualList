﻿using CiccioSoft.VirtualList.Data.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CiccioSoft.VirtualList.Data.Database;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ConfigureServiceProvider();
            //var db = Ioc.Default.GetService<DatabaseSerice>();
            //db.LoadSample(10000);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Assicurarsi che la finestra corrente sia attiva
                Window.Current.Activate();
            }
        }

        private void ConfigureServiceProvider()
        {
            // Crea IConfiguration
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            // Configura il ServiceProvider di Ioc.Default
            Ioc.Default.ConfigureServices(new ServiceCollection()

                // Aggiungi Configurazione
                .AddSingleton(configuration)

                // Aggiungi Configurazione Logger
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddDebug();
                })
                 
                // aggiungi data
                .AddData(configuration)

                // Aggiungi servizi
                //.AddTransient<ModelVirtualCollection>()
                .AddTransient<MainViewModel>()

                // Build ServiceProvider
                .BuildServiceProvider());
        }
    }
}
