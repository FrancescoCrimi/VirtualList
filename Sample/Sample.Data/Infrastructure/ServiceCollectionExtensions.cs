using CiccioSoft.VirtualList.Sample.Database;
using CiccioSoft.VirtualList.Sample.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CiccioSoft.VirtualList.Sample.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection serviceCollection,
                                                 IConfiguration configuration)
        {
            var section = configuration.GetSection("MyDbType");
            DbType dbt = Enum.Parse<DbType>(section.Value);

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

            return serviceCollection;
        }
    }
}