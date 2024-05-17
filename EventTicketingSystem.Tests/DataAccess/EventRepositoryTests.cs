using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    internal class EventRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IEventRepository _eventRepository;
        private DatabaseContext _context;
        private TestDataSeeder _seeder;

        public EventRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _eventRepository = new EventRepository(_context);
            _seeder = new TestDataSeeder(_context, _dataProvider);
            await _seeder.SeedCities();
            await _seeder.SeedVenues();
            await _seeder.SeedSeats(1);
            await _seeder.SeedEvents();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task EventRepository_Find_ReturnsEvent()
        {
            var expectedEvent = _dataProvider.GetEvents()[0].EventOccurrences.First();

            var foundEvent = await _eventRepository.Find(1);

            foundEvent.Should().BeEquivalentTo(expectedEvent, ExcludeProperties);
        }

        [Test]
        public async Task EventRepository_GetEvents_ReturnsEvents()
        {
            var expectedEvents = _dataProvider.GetEvents().SelectMany(x => x.EventOccurrences).ToList();

            var events = await _eventRepository.GetEvents();

            events.Should().BeEquivalentTo(expectedEvents, ExcludeProperties);
        }

        [Test]
        public async Task EventRepository_GetEventsByCity_ReturnsEvents()
        {
            var expectedEvents = _dataProvider.GetEvents()
                .SelectMany(x => x.EventOccurrences)
                .Where(x => x.VenueId == 1)
                .ToList();

            var events = await _eventRepository.GetEventsByCity(1);

            events.Should().BeEquivalentTo(expectedEvents, ExcludeProperties);
        }

        [Test]
        public async Task EventRepository_GetEventsByVenue_ReturnsEvents()
        {
            var expectedEvents = _dataProvider.GetEvents()
                .SelectMany(x => x.EventOccurrences)
                .Where(x => x.VenueId == 4)
                .ToList();

            var events = await _eventRepository.GetEventsByVenue(4);

            events.Should().ContainSingle();
            events.Should().BeEquivalentTo(expectedEvents, ExcludeProperties);
        }


        private EquivalencyAssertionOptions<Event> ExcludeProperties(EquivalencyAssertionOptions<Event> options)
        {
            options.Excluding(o => o.Venue);
            options.Excluding(o => o.EventInfo);
            options.Excluding(o => o.Id);
            return options;
        }
    }
}
