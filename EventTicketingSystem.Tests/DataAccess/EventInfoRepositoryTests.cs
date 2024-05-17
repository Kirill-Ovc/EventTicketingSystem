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
    internal class EventInfoRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IEventInfoRepository _eventInfoRepository;
        private DatabaseContext _context;
        private TestDataSeeder _seeder;

        public EventInfoRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _eventInfoRepository = new EventInfoRepository(_context);
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
        public async Task EventInfoRepository_Find_ReturnsEventInfo()
        {
            var expectedEventInfo = _dataProvider.GetEvents()[0];

            var eventInfo = await _eventInfoRepository.Find(1);

            eventInfo.Should().BeEquivalentTo(expectedEventInfo, ExcludeProperties);
        }

        [Test]
        public async Task EventInfoRepository_Delete_DeletesEventInfo()
        {
            var eventInfoId = 1;

            await _eventInfoRepository.Delete(eventInfoId);
            await _context.SaveChangesAsync();

            var deletedEventInfo = await _eventInfoRepository.Find(eventInfoId);
            deletedEventInfo.Should().BeNull();
        }

        [Test]
        public async Task EventInfoRepository_GetEvents_ReturnsEvents()
        {
            var expectedEvents = _dataProvider.GetEvents();

            var events = await _eventInfoRepository.GetEvents();

            events.Should().BeEquivalentTo(expectedEvents, ExcludeProperties);
        }

        [Test]
        public async Task EventInfoRepository_GetEventsByCity_ReturnsEvents()
        {
            var expectedEvents = _dataProvider.GetEvents().Take(2);

            var events = await _eventInfoRepository.GetEventsByCity(1);

            events.Should().BeEquivalentTo(expectedEvents, ExcludeProperties);
        }

        [Test]
        public async Task EventInfoRepository_GetEventsByVenue_ReturnsEvents()
        {
            var expectedEvent = _dataProvider.GetEvents()[2];

            var events = await _eventInfoRepository.GetEventsByVenue(4);

            events.Should().ContainSingle();
            events.First().Should().BeEquivalentTo(expectedEvent, ExcludeProperties);
        }

        private EquivalencyAssertionOptions<EventInfo> ExcludeProperties(EquivalencyAssertionOptions<EventInfo> options)
        {
            options.For(o => o.EventOccurrences).Exclude(o => o.Venue);
            options.For(o => o.EventOccurrences).Exclude(o => o.EventInfo);
            options.For(o => o.EventOccurrences).Exclude(o => o.Id);
            return options;
        }
    }
}
