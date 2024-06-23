using EventTicketingSystem.DataAccess.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Tests.Integration.Seed
{
    internal class TestDataSeeder
    {
        private readonly DatabaseContext _context;
        private readonly TestDataProvider _dataProvider;

        public TestDataSeeder(
            DatabaseContext context,
            TestDataProvider dataProvider)
        {
            _context = context;
            _dataProvider = dataProvider;
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

        public async Task SeedOffers(int venueId, int eventId)
        {
            if (await _context.Offers.AnyAsync()) return;

            var sections = await _context.Sections
                .Where(s => s.VenueId == venueId)
                .OrderBy(s => s.Id)
                .ToListAsync();
            var offers = _dataProvider.GetOffers(eventId, sections);
            await SeedEntities(offers);
        }

        public async Task SeedBookings()
        {
            if (await _context.Bookings.AnyAsync()) return;

            var bookings = _dataProvider.GetBookings();
            await SeedEntities(bookings);
        }

        public async Task SeedPayments()
        {
            if (await _context.Payments.AnyAsync()) return;

            var payments = _dataProvider.GetPayments();
            await SeedEntities(payments);
        }

        public async Task SeedTickets()
        {
            if (await _context.Tickets.AnyAsync()) return;

            var tickets = _dataProvider.GetTickets();
            await SeedEntities(tickets);
        }

        private async Task SeedEntities<T>(IEnumerable<T> entities) where T : class
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
