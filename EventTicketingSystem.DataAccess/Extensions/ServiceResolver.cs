﻿using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Settings;
using EventTicketingSystem.DataAccess.Seed;
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
            services.RegisterContext(configuration);
            services.RegisterRepositories();

            services.AddScoped<ISeedService, SeedService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        private static void RegisterContext(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            if (databaseSettings == null)
            {
                throw new ArgumentException($"Configuration section '{DatabaseSettings.SectionName}' is missing or has an invalid structure.");
            }

            services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));

            services.AddDbContext<DatabaseContext>(options => options.UseSqlite(databaseSettings.GetConnectionString()));
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
        }
    }
}