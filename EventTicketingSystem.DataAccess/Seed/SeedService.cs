using System.Text.Json;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Seed
{
    internal class SeedService : ISeedService
    {
        private readonly DatabaseContext _context;

        public SeedService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task SeedAllData()
        {
            await SeedCities();
            await SeedUsers();
        }

        public async Task SeedUsers()
        {
            if (await _context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Seed/Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<User>>(userData, options);

            foreach (var user in users)
            {
                user.Username = user.Username.ToLower();
                user.Password = "Password";
            }

            await _context.SaveChangesAsync();
        }

        public async Task SeedCities()
        {
            if (await _context.Cities.AnyAsync()) return;

            var cityData = await File.ReadAllTextAsync("Seed/Data/CitiesSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cities = JsonSerializer.Deserialize<List<City>>(cityData, options);

            await _context.AddRangeAsync(cities);
            await _context.SaveChangesAsync();
        }
    }
}
