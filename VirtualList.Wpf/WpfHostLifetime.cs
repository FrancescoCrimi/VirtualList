using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Wpf
{
    public class WpfHostLifetime : IHostLifetime
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHostEnvironment environment;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly ILogger logger;
        private CancellationTokenRegistration applicationStartedRegistration;
        private CancellationTokenRegistration applicationStoppingRegistration;

        public WpfHostLifetime(IServiceProvider serviceProvider,
                               IHostEnvironment environment,
                               IHostApplicationLifetime applicationLifetime,
                               ILoggerFactory loggerFactory)
        {
            this.serviceProvider = serviceProvider;
            this.environment = environment;
            this.applicationLifetime = applicationLifetime;
            logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            applicationStartedRegistration = applicationLifetime.ApplicationStarted.Register(state =>
            {
                ((WpfHostLifetime)state!).OnApplicationStarted();
            },
            this);
            applicationStoppingRegistration = applicationLifetime.ApplicationStopping.Register(state =>
            {
                ((WpfHostLifetime)state!).OnApplicationStopping();
            },
            this);

            RegisterShutdownHandlers();

            serviceProvider.GetRequiredService<MainView>().Show();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            logger.LogInformation("Application started.");
            logger.LogInformation("Hosting environment: {envName}", environment.EnvironmentName);
            logger.LogInformation("Content root path: {contentRoot}", environment.ContentRootPath);
        }

        private void OnApplicationStopping()
        {
            logger.LogInformation("Application is shutting down...");
        }

        private void RegisterShutdownHandlers()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            applicationLifetime.StopApplication();
            UnregisterShutdownHandlers();
            applicationStartedRegistration.Dispose();
            applicationStoppingRegistration.Dispose();
        }

        private void UnregisterShutdownHandlers()
        {
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        }
    }
}
