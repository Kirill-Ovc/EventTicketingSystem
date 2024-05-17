using Microsoft.Extensions.Configuration;

namespace EventTicketingSystem.ConsoleTest
{
    internal static class ConfigurationProvider
    {
        private static readonly IConfigurationRoot _config;
        static ConfigurationProvider()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.Development.json", false, false)
                .Build();
        }

        public static IConfiguration Configuration()
        {
            return _config;
        }
    }
}
