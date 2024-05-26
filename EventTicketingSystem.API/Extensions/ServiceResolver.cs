using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Services;

namespace EventTicketingSystem.API.Extensions
{
    public static class ServiceResolver
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();
        }
    }
}
