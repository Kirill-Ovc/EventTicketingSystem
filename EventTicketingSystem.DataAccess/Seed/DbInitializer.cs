using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventTicketingSystem.DataAccess.Seed
{
    /// <summary>
    /// Helper class to seed the database with initial data
    /// </summary>
    internal static class DbInitializer
    {
        /// <summary>
        /// Initialize the database with initial data
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(DatabaseContext context)
        {
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            SeedCities(context).Wait();
            SeedUsers(context).Wait();

            context.SaveChanges();
            context.Database.CloseConnection();
        }

        private static async Task SeedCities(DatabaseContext context)
        {
            if (await context.Cities.AnyAsync()) return;

            var cityData = await File.ReadAllTextAsync("Seed/Data/CitySeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cities = JsonSerializer.Deserialize<List<City>>(cityData, options);

            await context.AddRangeAsync(cities);
        }

        private static async Task SeedUsers(DatabaseContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    Username = "admin",
                    Name = "Admin",
                    Password = "password",
                    Email = "admin@email"
                },
                new()
                {
                    Id = 2,
                    Username = "TestUser",
                    Name = "Test User",
                    Password = "password",
                    Email = "test@email"
                },
                new()
                {
                    Id = 3,
                    Username = "Kirill1",
                    Name = "Kirill O",
                    Password = "password",
                    Email = "kirill@email"
                },
            };

            await context.AddRangeAsync(users);
        }
    }
}
