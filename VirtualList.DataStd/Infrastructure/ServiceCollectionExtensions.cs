using CiccioSoft.VirtualList.Data.Database;
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
                .AddTransient<DatabaseSerice>();

            return serviceCollection;
        }
    }
}