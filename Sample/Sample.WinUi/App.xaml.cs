﻿using CiccioSoft.VirtualList.Sample.Database;
using CiccioSoft.VirtualList.Sample.WinUi.ViewModels;
using CiccioSoft.VirtualList.Sample.WinUi.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
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

        // Crea Configurazione           
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddLogging(loggingBuilder =>
                loggingBuilder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    //.AddNLog()
                    .AddDebug())
        
            // aggiungi data
            .AddData(configuration)

            // Views and ViewModels
            .AddTransient<ListViewViewModel>()
            .AddTransient<ListViewPage>()

            .BuildServiceProvider());
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window m_window = new MainView
        {
            //Content = new ListViewPage()
        };
        m_window.Activate();

        //Ioc.Default.GetRequiredService<DatabaseSerice>().LoadSample(10000);
    }
}
