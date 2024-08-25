using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.Contract.Models;

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
    }
}
