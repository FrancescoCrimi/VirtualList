using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Windows;

namespace CiccioSoft.VirtualList.Wpf
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //await CreateHostBuilder(e.Args).StartAsync();

            ConfigureServiceProvider();
            await Application.Current.Dispatcher.InvokeAsync(() =>
                Ioc.Default.GetRequiredService<MainView>().Show());
        }

        private void ConfigureServiceProvider()
        {
            // Crea Configurazione
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables(prefix: "DOTNET_");
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            string environment = builder.Build().GetValue<string>("ENVIRONMENT");
            builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);     
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
                        .AddDebug()
                        .AddEventLog();
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
