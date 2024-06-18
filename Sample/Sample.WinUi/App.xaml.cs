// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Database;
using CiccioSoft.VirtualList.Sample.WinUi.Repository;
using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using CiccioSoft.VirtualList.Sample.WinUi.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using System;

namespace CiccioSoft.VirtualList.Sample.WinUi;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ConfigureServices();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window m_window = new MainView();
        m_window.Activate();
    }

    private void ConfigureServices()
    {
        // Configuration           
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();

        // Logging
        serviceCollection.AddLogging(builder => builder
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddDebug());

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

        // Views and ViewModels
        serviceCollection
            .AddTransient<ListViewViewModel>()
            .AddTransient<ListViewPage>();

        // Ioc.Default
        Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
    }
}
