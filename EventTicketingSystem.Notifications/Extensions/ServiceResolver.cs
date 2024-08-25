using EventTicketingSystem.Contract.Models;
using EventTicketingSystem.Notifications.Interfaces;
using EventTicketingSystem.Notifications.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventTicketingSystem.Notifications.Extensions
{
    public static class ServiceResolver
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        public static void AddMessaging(this IHostBuilder builder, IConfiguration configuration)
        {
            var endpointName = "EventTicketingSystem_Notifications";
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology(QueueType.Classic);
            
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(SendNotification), endpointName);
            var connectionString = configuration.GetValue<string>("MessagingSettings:ConnectionString");
            transport.ConnectionString(connectionString);

            builder.UseNServiceBus(_ => endpointConfiguration);
        }
    }
}
