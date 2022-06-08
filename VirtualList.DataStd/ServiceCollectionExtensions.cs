using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CiccioSoft.VirtualList.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection serviceCollection,
                                                 IConfiguration configuration)
        {
            var option = configuration.GetSection("MyDbType");
            DbType dbt = (DbType)Enum.Parse(typeof(DbType), option.Value);
            switch (dbt)
            {
                case DbType.SqLite:
                    serviceCollection
                        .AddDbContext<SqLiteDbContext>(options =>
                        {
                            options
                                //.UseLazyLoadingProxies()
                                .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                                .UseSqlite(configuration.GetConnectionString("SqLiteConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider) => serviceProvider.GetRequiredService<SqLiteDbContext>());
                    break;
                case DbType.MsLocalDb:
                    serviceCollection
                        .AddDbContext<SqlServerDbContext>(options =>
                        {
                            options
                                //.UseLazyLoadingProxies()
                                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider) => serviceProvider.GetRequiredService<SqlServerDbContext>());
                    break;
                default:
                    break;
            }
            serviceCollection
                .AddTransient<IModelRepository, ModelRepository>();
            return serviceCollection;
        }
    }
}