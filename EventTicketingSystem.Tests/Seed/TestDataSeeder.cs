using EventTicketingSystem.DataAccess.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Tests.Seed
{
    internal class TestDataSeeder
    {
        private readonly DatabaseContext _context;
        private readonly TestDataReader _dataProvider;

        public TestDataSeeder(
            DatabaseContext context, 
            TestDataReader dataProvider)
        {
            _context = context;
            _dataProvider = dataProvider;
        }

        public async Task SeedCities()
        {
            if (await _context.Cities.AnyAsync()) return;

            var cities = await _dataProvider.GetCities();
            await SeedEntities(cities);
        }

        private async Task SeedEntities<T>(List<T> entities) where T : class
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
