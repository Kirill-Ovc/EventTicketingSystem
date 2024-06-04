using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.API.Services;

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
        }

        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        }

    }
}
