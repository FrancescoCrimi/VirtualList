using CiccioSoft.VirtualList.Data.Database;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Infrastructure;
using CiccioSoft.VirtualList.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.LoadData
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await new Program().StartAsync();
        }

        private async Task StartAsync()
        {
            try
            {
                var serviceProvider = CreateServiceProvider();
                var dbContext = serviceProvider.GetService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                FakeModelRepository repo = new FakeModelRepository();

                for (int i = 0; i < 1000000; i++)
                {
                    Model model = repo.GetAll()[i];
                    dbContext.Add(model);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private IServiceProvider CreateServiceProvider()
        {
            // Crea ServiceCollection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Crea Configurazione
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            return serviceCollection

                // Aggiungi Configurazione Logger
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddDebug()
                        .AddConsole();
                })

                // aggiungi data
                .AddData(configuration)
                .BuildServiceProvider();
        }
    }
}
