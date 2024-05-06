using EventTicketingSystem.DataAccess.Models.Entities;
using System.Text.Json;

namespace EventTicketingSystem.Tests.Seed
{
    internal class TestDataReader
    {
        public async Task<List<User>> GetUsers()
        {
            var userData = await File.ReadAllTextAsync("Seed/TestData/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<User>>(userData, options);
            return users;
        }

        public async Task<List<City>> GetCities()
        {
            var cityData = await File.ReadAllTextAsync("Seed/TestData/CitySeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cities = JsonSerializer.Deserialize<List<City>>(cityData, options);
            return cities;
        }
    }
}
