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
            serviceCollection.Configure<MyAppOptions>(configuration.GetSection("MyAppOptions"));

            var option = configuration.GetSection("MyDbType");
            DbType dbt = Enum.Parse<DbType>(option.Value);


            switch (dbt)
            {
                case DbType.SqLite:
                    break;
                case DbType.MsLocalDb:
                    serviceCollection
                        .AddDbContext<SqlServerDbContext>(options =>
                        {
                            options
                                //.UseLazyLoadingProxies()
                                .ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider) => serviceProvider.GetRequiredService<SqlServerDbContext>());
                    break;
            }

            serviceCollection
                .AddTransient<IModelRepository, ModelRepository>()
                .AddTransient<LoadSampleSerice>();

            return serviceCollection;
        }
    }
}