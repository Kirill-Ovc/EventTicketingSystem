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

        public async Task SeedUsers()
        {
            if (await _context.Users.AnyAsync()) return;

            var users = await _dataProvider.GetUsers();
            await SeedEntities(users);
        }

        public async Task SeedVenues()
        {
            if (await _context.Venues.AnyAsync()) return;

            var venues = _dataProvider.GetVenues();
            await SeedEntities(venues);
        }

        public async Task SeedSeats(int venueId)
        {
            if (await _context.Seats.AnyAsync()) return;

            var sections = _dataProvider.GetSectionsWithSeats(venueId);
            await SeedEntities(sections);
        }

        public async Task SeedEvents()
        {
            if (await _context.Events.AnyAsync()) return;

            var events = _dataProvider.GetEvents();
            await SeedEntities(events);
        }

        public async Task SeedEventSeats(int venueId, int eventId)
        {
            if (await _context.EventSeats.AnyAsync()) return;

            var venueSeats = await _context.Seats
                .Where(s => s.VenueId == venueId)
                .OrderBy(s => s.Id)
                .ToListAsync();
            var eventSeats = _dataProvider.GetEventSeats(venueSeats, eventId);
            await SeedEntities(eventSeats);
        }

        private async Task SeedEntities<T>(IEnumerable<T> entities) where T : class
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
