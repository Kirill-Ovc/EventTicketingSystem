using EventTicketingSystem.DataAccess.Helpers;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Settings;
using EventTicketingSystem.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventTicketingSystem.DataAccess.Extensions
{
    /// <summary>
    /// Class for registering services
    /// </summary>
    public static class ServiceResolver
    {
        /// <summary>
        /// Registers services and settings for Data Access. 
        /// </summary>
        /// <param name="services">Collection of registered services</param>
        /// <param name="configuration">Application configuration properties with configuration section for <see cref="EmployeeIdentificationSettings"/></param>
        public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            services.RegisterContext(configuration);
            services.RegisterRepositories();

            services.AddAutoMapper(typeof(MapperProfile));
        }

        /// <summary>
        /// Initialize Database with initial data.
        /// Call after services are registered and app is configured.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InitializeDatabase(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<DatabaseContext>();
            if (dbContext is null)
            {
                throw new InvalidOperationException("Database Context is not registered. AddDataAccess() method was not called");
            }
            DbInitializer.Initialize(dbContext);
        }

        private static void RegisterContext(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            if (databaseSettings is null)
            {
                throw new ArgumentException($"Configuration section '{DatabaseSettings.SectionName}' is missing or has an invalid structure.");
            }

            services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));

            services.AddDbContext<DatabaseContext>(options =>
            {
                if (IsDevelopment())
                {
                    options.UseSqlite(databaseSettings.ConnectionString);
                }
                else
                {
                    options.UseSqlServer(databaseSettings.ConnectionString);
                }
            });
        }

        private static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IEventInfoRepository, EventInfoRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<IBookingSeatRepository, BookingSeatRepository>();
        }

        private static bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }
    }
}
