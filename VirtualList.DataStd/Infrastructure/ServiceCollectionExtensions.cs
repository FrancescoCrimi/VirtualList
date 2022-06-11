using CiccioSoft.VirtualList.DataStd.Database;
using CiccioSoft.VirtualList.DataStd.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CiccioSoft.VirtualList.DataStd.Infrastructure
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

                    serviceCollection.AddTransient<IModelRepository, ModelRepository>();

                    break;

                case DbType.MsLocalDb:

                    serviceCollection.AddDbContext<SqlServerDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SqlServerDbContext>());

                    serviceCollection.AddTransient<IModelRepository, ModelRepository>();

                    break;

                case DbType.FakeDb:

                    serviceCollection.AddSingleton<IModelRepository, FakeModelRepository>();

                    break;
            }
            return serviceCollection;
        }
    }
}