using EventTicketingSystem.API.Helpers;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.Contract.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventTicketingSystem.API.Extensions
{
    public static class ServiceResolver
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBookingSeatService, BookingSeatService>();
            services.AddScoped<IBookingCartMapper, BookingCartMapper>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        }

        public static void AddMessaging(this WebApplicationBuilder builder)
        {
            var endpointName = "EventTicketingSystem_Notifications";
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<SystemJsonSerializer>();
            // Comment SendOnly to create exchanges and queues automatically with the first run
            endpointConfiguration.SendOnly();
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology(QueueType.Classic);
            
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(SendNotification), endpointName);

            var connectionString = builder.Configuration.GetValue<string>("MessagingSettings:ConnectionString");
            transport.ConnectionString(connectionString);

            builder.Host.UseNServiceBus(_ => endpointConfiguration);
        }

        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck<VersionHealthCheck>("apiVersion", failureStatus: HealthStatus.Unhealthy)
                .AddSqlServer(
                    connectionString: configuration.GetValue<string>("DatabaseSettings:ConnectionString"),
                    name: "sqlserver",
                    healthQuery: "SELECT 1;", // Optional: A custom SQL query to check database health
                    failureStatus: HealthStatus.Unhealthy)
                .AddRabbitMQ(
                    rabbitConnectionString: configuration.GetValue<string>("MessagingSettings:ConnectionString"),
                    name: "rabbitmq",
                    failureStatus: HealthStatus.Unhealthy);
        }
    }
}
