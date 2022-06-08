using CiccioSoft.VirtualList.Data;
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
                var loadSample = serviceProvider.GetService<LoadSampleSerice>();
                await loadSample.LoadSample();
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
                        .AddConsole();
                })

                // aggiungi data
                .AddData(configuration)

                .AddTransient<LoadSampleSerice>()

                .BuildServiceProvider();
        }
    }
}
