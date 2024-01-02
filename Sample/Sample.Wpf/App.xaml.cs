using CiccioSoft.VirtualList.Sample.Infrastructure;
using CiccioSoft.VirtualList.Sample.Wpf.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace CiccioSoft.VirtualList.Sample.Wpf;

public partial class App : Application
{
    public App()
    {
        // Crea Configurazione
        var builder = new ConfigurationBuilder();
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
            })

            .AddData(configuration)

            //.AddTransient<MainViewModel>()
            //.AddTransient<MainView>()

            .BuildServiceProvider());
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            //Ioc.Default.GetRequiredService<MainView>().Show();
            new MainView().Show();
        });
        //await Application.Current.Dispatcher.InvokeAsync(() =>
        //{
        //    Ioc.Default.GetRequiredService<DatabaseSerice>().LoadSample(1000000);
        //    Application.Current.Shutdown();
        //});
    }
}
