using EventTicketingSystem.DataAccess.Extensions;
using EventTicketingSystem.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventTicketingSystem.ConsoleTest
{
    internal static class ServiceProvider
    {
        private static readonly IServiceProvider _serviceProvider;

        static ServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var config = ConfigurationProvider.Configuration();
            services.AddDataAccess(config);
        }

        public static IUserRepository GetUserRepository()
        {
            var service = _serviceProvider.GetService<IUserRepository>();
            return service;
        }

        public static ISeatRepository GetSeatRepository()
        {
            var service = _serviceProvider.GetService<ISeatRepository>();
            return service;
        }

        public static ISeedService GetSeedService()
        {
            var service = _serviceProvider.GetService<ISeedService>();
            return service;
        }

    }
}
