using CiccioSoft.VirtualList.Data.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Windows;
using CiccioSoft.VirtualList.Data.Database;

namespace CiccioSoft.VirtualList.Wpf
{
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //await CreateHostBuilder(e.Args).StartAsync();

            ConfigureServiceProvider();

            await Application.Current.Dispatcher.InvokeAsync(() =>
                Ioc.Default.GetRequiredService<MainView>().Show());
            //await Application.Current.Dispatcher.InvokeAsync(() =>
            //{
            //    Ioc.Default.GetRequiredService<DatabaseSerice>().LoadSample(100000);
            //    Application.Current.Shutdown();
            //});
        }

        private void ConfigureServiceProvider()
        {
            // Crea Configurazione
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
                        //.AddNLog()
                        .AddDebug();
                    //.AddEventLog();
                })

                // aggiungi data
                .AddData(configuration)

                // Aggiungi servizi
                .AddTransient<MainViewModel>()
                .AddTransient<MainView>()

                // Build ServiceProvider
                .BuildServiceProvider());
        }

        private IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection
                        .AddData(hostBuilderContext.Configuration)
                        .AddSingleton<IHostLifetime, WpfHostLifetime>()
                        .AddTransient<MainViewModel>()
                        .AddTransient<MainView>();
                });
        }
    }
}
