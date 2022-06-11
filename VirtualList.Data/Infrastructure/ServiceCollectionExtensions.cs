﻿using CiccioSoft.VirtualList.Data.Database;
using CiccioSoft.VirtualList.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CiccioSoft.VirtualList.Data.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection serviceCollection,
                                                 IConfiguration configuration)
        {
            serviceCollection.Configure<MyAppOptions>(configuration.GetSection("MyAppOptions"));

            var option = configuration.GetSection("MyDbType");
            DbType dbt = Enum.Parse<DbType>(option.Value);


            switch (dbt)
            {
                case DbType.SqLite:
                    serviceCollection.AddDbContext<SqLiteDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlite(configuration.GetConnectionString("SqLiteConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SqLiteDbContext>());

                    serviceCollection
                        .AddTransient<IModelRepository, ModelRepository>();

                    break;

                case DbType.MsLocalDb:

                    serviceCollection.AddDbContext<SqlServerDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SqlServerDbContext>());

                    serviceCollection
                        .AddTransient<IModelRepository, ModelRepository>();

                    break;

                case DbType.FakeDb:

                    serviceCollection
                        .AddSingleton<IModelRepository, FakeModelRepository>();

                    break;
            }

            serviceCollection
                .AddTransient<LoadSampleSerice>();

            return serviceCollection;
        }
    }
}