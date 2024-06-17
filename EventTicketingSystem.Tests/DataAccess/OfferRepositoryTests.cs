using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class OfferRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IOfferRepository _offerRepository;
        private DatabaseContext _context;
        private const int EventId = 1;
        private const int VenueId = 1;

        public OfferRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _offerRepository = new OfferRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedUsers();
            await seeder.SeedVenues();
            await seeder.SeedSeats(VenueId);
            await seeder.SeedOffers(VenueId, EventId);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task OfferRepository_GetOffersByEvent_ReturnsOffers()
        {
            // Act
            var offers = await _offerRepository.GetOffersByEvent(EventId);

            // Assert
            offers.Count.Should().Be(3);
            offers.Should().BeEquivalentTo(_expectedOffers, options => 
                options.Excluding(x => x.Event).Excluding(x => x.Section));
        }

        private readonly List<Offer> _expectedOffers = new List<Offer>()
        {
            new Offer()
            {
                Id = 1,
                EventId = EventId,
                SectionId = 1,
                RowNumber = 1,
                TicketLevel = TicketLevel.Other,
                Price = 100
            },
            new Offer()
            {
                Id = 2,
                EventId = EventId,
                SectionId = 2,
                RowNumber = 1,
                TicketLevel = TicketLevel.Other,
                Price = 100
            },
            new Offer()
            {
                Id = 3,
                EventId = EventId,
                SectionId = 3,
                RowNumber = 1,
                TicketLevel = TicketLevel.Other,
                Price = 100
            }
        };
    }
}
