//using CiccioSoft.VirtualList.DataStd.Infrastructure;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class App : Application
    {
        public App()
        {
            //ConfigureServiceProvider();
        }

        //private void ConfigureServiceProvider()
        //{
        //    // Crea IConfiguration
        //    Microsoft.Extensions.Configuration.ConfigurationBuilder builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        //    builder.AddEnvironmentVariables(prefix: "DOTNET_");
        //    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    string environment = builder.Build().GetValue<string>("ENVIRONMENT");
        //    builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        //    IConfiguration configuration = builder.Build();

        //    // Configura il ServiceProvider di Ioc.Default
        //    Ioc.Default.ConfigureServices(new ServiceCollection()

        //        // Aggiungi Configurazione
        //        .AddSingleton(configuration)

        //        // Aggiungi Configurazione Logger
        //        .AddLogging(loggingBuilder =>
        //        {
        //            loggingBuilder
        //                .AddConfiguration(configuration.GetSection("Logging"))
        //                .AddNLog()
        //                .AddDebug();
        //            //.AddEventLog();
        //        })

        //        // aggiungi data
        //        //.AddData(configuration)

        //        //// Aggiungi servizi
        //        //.AddTransient<ModelVirtualCollection>()
        //        //.AddTransient<MainViewModel>()

        //        // Build ServiceProvider
        //        .BuildServiceProvider());
        //}
    }
}
