using System;
using CiccioSoft.VirtualList.Sample.Uwp.Database;
using CiccioSoft.VirtualList.Sample.Uwp.Repository;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.VirtualList.Sample.Uwp
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

            var serviceCollection = new ServiceCollection()

            // Aggiungi Configurazione
            .AddSingleton(configuration)

            // Aggiungi Configurazione Logger
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddDebug();
            });

            var section = configuration.GetSection("MyDbType");
            DbType dbt = (DbType)Enum.Parse(typeof(DbType), section.Value);
            switch (dbt)
            {
                // Use SqLite
                case DbType.SqLite:
                    serviceCollection
                        .AddDbContext<AppDbContext>(options =>
                        {
                            options
                                //.UseLazyLoadingProxies()
                                .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                                .UseSqlite(configuration.GetConnectionString("SqLiteConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                        .AddTransient<IModelRepository, ModelRepository>();
                    break;

                // MS LocalDB
                case DbType.MsLocalDb:
                    break;

                // MS SqlServer
                case DbType.SqlServer:
                    serviceCollection
                        .AddDbContext<AppDbContext>(options =>
                        {
                            options
                                //.UseLazyLoadingProxies()
                                .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                                .UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                        .AddTransient<IModelRepository, ModelRepository>();
                    break;

                // MySql
                case DbType.MySql:
                    serviceCollection
                        .AddDbContext<AppDbContext>(options =>
                        {
                            options
                                .UseMySql(configuration.GetConnectionString("MySqlConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                        .AddTransient<IModelRepository, ModelRepository>();
                    break;

                // Fake Repo
                case DbType.FakeDb:
                    serviceCollection
                        .AddSingleton<IModelRepository, FakeModelRepository>();
                    break;
            }

            serviceCollection
                .AddTransient<DatabaseSerice>()            
                .AddTransient<MainViewModel>();

            Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());            
        }

    }
}
