// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Wpf.Database;
using CiccioSoft.VirtualList.Sample.Wpf.Repository;
using CiccioSoft.VirtualList.Sample.Wpf.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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



        var serviceCollection = new ServiceCollection()

        //// Aggiungi Configurazione
        //.AddSingleton(configuration)

        // Aggiungi Configurazione Logger
        .AddLogging(loggingBuilder =>
        {
            loggingBuilder
                .AddConfiguration(configuration.GetSection("Logging"))
                //.AddNLog()
                .AddDebug();
        });

        //.AddData(configuration)

        var section = configuration.GetSection("MyDbType");
        DbType dbt = Enum.Parse<DbType>(section.Value!);

        switch (dbt)
        {
            // Use SqLite
            case DbType.SqLite:
                serviceCollection
                    .AddDbContext<AppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlite(configuration.GetConnectionString("SqLiteConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                    .AddTransient<IModelRepository, ModelRepository>();
                break;

            // MS LocalDB
            case DbType.MsLocalDb:
                serviceCollection
                    .AddDbContext<AppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()                            
                            //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlServer(configuration.GetConnectionString("MsLocalDbConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                    .AddTransient<IModelRepository, ModelRepository>();
                break;

            // MS SqlServer
            case DbType.SqlServer:
                serviceCollection
                    .AddDbContext<AppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                    .AddTransient<IModelRepository, ModelRepository>();
                break;

            case DbType.MySql:
                var connectionString = configuration.GetConnectionString("MySqlConnection");
                var serverVersion = new MariaDbServerVersion(new Version(10, 11, 5));
                serviceCollection
                    .AddDbContext<AppDbContext>(options =>
                    {
                        options
                            //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseMySql(connectionString, serverVersion);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient)
                    .AddTransient<IModelRepository, ModelRepository>();
                break;

            // Fake Repo
            case DbType.FakeDb:
                serviceCollection
                    .AddSingleton<IModelRepository, FakeModelRepository>();
                break;
        }

        //.AddTransient<MainViewModel>()
        //.AddTransient<MainView>()

        // Configura il ServiceProvider di Ioc.Default
        Ioc.Default.ConfigureServices(serviceCollection
            .BuildServiceProvider());
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            //Ioc.Default.GetRequiredService<MainView>().Show();
            new MainView().Show();
        });

        //await Application.Current.Dispatcher.InvokeAsync(async () =>
        //{
        //    AppDbContext dbContext = Ioc.Default.GetRequiredService<AppDbContext>();
        //    var items = await dbContext.Models.ToListAsync();
        //    dbContext.Dispose();
        //    Application.Current.Shutdown();
        //});
    }
}
