using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using NLog.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Uwp
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            ConfigureServiceProvider();
            //using (var db = Ioc.Default.GetService<MyDbContext>())
            //{
            //    db.Database.Migrate();
            //}
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
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
                        .AddNLog()
                        .AddDebug();
                        //.AddEventLog();
                })

                // aggiungi data
                .AddData(configuration)

                // Aggiungi servizi
                .AddTransient<ModelVirtualCollection>()
                .AddTransient<MainViewModel>()

                // Build ServiceProvider
                .BuildServiceProvider());
        }
    }
}
