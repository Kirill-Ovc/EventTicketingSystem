using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class SeatRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private ISeatRepository _seatRepository;
        private DatabaseContext _context;
        private TestDataSeeder _seeder;

        public SeatRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _seatRepository = new SeatRepository(_context);
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
        public async Task SeatRepository_Find_ReturnsSeat()
        {
            var expectedSeat = new Seat()
            {
                Id = 1,
                Name = "A1-1",
                RowNumber = 1,
                SectionId = 1,
                VenueId = 1
            };
            var seat = await _seatRepository.Find(1);

            seat.Should().BeEquivalentTo(expectedSeat, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_Add_AddsSeat()
        {
            var seat = new Seat()
            {
                Name = "New Seat",
                RowNumber = 1,
                SectionId = 1,
                VenueId = 1
            };

            await _seatRepository.Add(seat);
            await _seatRepository.SaveChanges();

            var addedSeat = await _seatRepository.Find(seat.Id);

            addedSeat.Should().BeEquivalentTo(seat, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_Update_UpdatesSeat()
        {
            var seat = await _seatRepository.Find(1);
            seat.Name = "Updated Name";
            seat.RowNumber = 5;

            await _seatRepository.Update(seat);
            await _seatRepository.SaveChanges();

            var updatedSeat = await _seatRepository.Find(seat.Id);

            updatedSeat.Should().BeEquivalentTo(seat, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_Delete_DeletesSeat()
        {
            var existingSeat = await _seatRepository.Find(1);

            Assert.IsNotNull(existingSeat);

            await _seatRepository.Delete(1);
            await _seatRepository.SaveChanges();

            var deletedSeat = await _seatRepository.Find(1);

            deletedSeat.Should().BeNull();
        }

        [Test]
        public async Task SeatRepository_GetSections_ReturnsAllSectionsInVenue()
        {
            var venueId = 1;
            var expectedSections = _dataProvider.GetSectionsWithSeats(venueId);
            var sections = (await _seatRepository.GetSections(venueId)).OrderBy(x => x.Id).ToList();

            sections.Should().BeEquivalentTo(expectedSections, options =>
                options.Excluding(o => o.Venue).Excluding(o => o.Seats));
        }

        [Test]
        public async Task SeatRepository_GetEventSeats_ReturnsSeats()
        {
            var eventId = 1;
            var venueId = 1;
            await _seeder.SeedEventSeats(venueId, eventId);
            var venueSeats = (await _seatRepository.GetVenueSeats(venueId)).OrderBy(x => x.Id).ToList();
            var expectedSeats = _dataProvider.GetEventSeats(venueSeats, eventId);
            var seats = (await _seatRepository.GetEventSeats(eventId)).OrderBy(x => x.Id).ToList();

            seats.Should().BeEquivalentTo(expectedSeats, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_GetEventSeatsForSection_ReturnsSeats()
        {
            var eventId = 1;
            var venueId = 1;
            var sectionId = 1;
            await _seeder.SeedEventSeats(venueId, eventId);
            var venueSeats = (await _seatRepository.GetVenueSeats(venueId))
                .Where(x => x.SectionId == sectionId)
                .OrderBy(x => x.Id)
                .ToList();
            var expectedSeats = _dataProvider.GetEventSeats(venueSeats, eventId);

            var seats = (await _seatRepository.GetEventSeats(eventId, sectionId)).OrderBy(x => x.Id).ToList();

            seats.Should().BeEquivalentTo(expectedSeats, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_GetVenueSeats_ReturnsSeats()
        {
            var venueId = 1;
            var expectedSeats = _dataProvider.GetSectionsWithSeats(venueId)
                .SelectMany(x => x.Seats)
                .ToList();

            var seats = (await _seatRepository.GetVenueSeats(venueId)).OrderBy(x => x.Id).ToList();

            seats.Should().BeEquivalentTo(expectedSeats, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_CreateEventSeats_CreatesEventSeats()
        {
            var eventId = 1;
            var venueId = 1;
            var venueSeats = (await _seatRepository.GetVenueSeats(venueId)).OrderBy(x => x.Id).ToList();
            var expectedEventSeats = _dataProvider.GetEventSeats(venueSeats, eventId);

            await _seatRepository.CreateEventSeats(eventId, venueId);
            await _seatRepository.SaveChanges();

            var createdEventSeats = (await _seatRepository.GetEventSeats(eventId)).OrderBy(x => x.Id).ToList();

            createdEventSeats.Should().BeEquivalentTo(expectedEventSeats, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_UpdateEventSeat_UpdatesEventSeat()
        {
            var eventId = 1;
            var venueId = 1;
            await _seeder.SeedEventSeats(venueId, eventId);
            var allEventSeats = await _seatRepository.GetEventSeats(eventId);
            var eventSeat = allEventSeats.First();
            eventSeat.Status = EventSeatStatus.Booked;

            await _seatRepository.UpdateEventSeat(eventSeat);
            await _seatRepository.SaveChanges();

            allEventSeats = await _seatRepository.GetEventSeats(eventId);
            var updatedEventSeat = allEventSeats.FirstOrDefault(x => x.Id == eventSeat.Id);

            updatedEventSeat.Should().BeEquivalentTo(eventSeat, ExcludeProperties);
        }

        [Test]
        public async Task SeatRepository_Find_WhenNotExist_ReturnsNull()
        {
            var seat = await _seatRepository.Find(1000);
            seat.Should().BeNull();
        }

        [Test]
        public async Task SeatRepository_Update_WhenNotExist_ThrowsException()
        {
            var newSeat = new Seat
            {
                Id = 2000,
                Name = "Seat A20",
                RowNumber = 2,
                SectionId = 1,
                VenueId = 1
            };

            Assert.That(async () =>
            {
                await _seatRepository.Update(newSeat);
                await _seatRepository.SaveChanges();
            }, Throws.Exception);

            var updatedSeat = await _seatRepository.Find(newSeat.Id);
            updatedSeat.Should().BeNull();
        }
        
        [Test]
        public void SeatRepository_UpdateEventSeat_WhenNotExist_ThrowsException()
        {
            var newEventSeat = new EventSeat
            {
                Id = 2000,
                EventId = 1,
                SeatId = 1,
                Status = EventSeatStatus.Booked
            };

            Assert.That(async () =>
            {
                await _seatRepository.UpdateEventSeat(newEventSeat);
            }, Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void SeatRepository_Delete_WhenSeatDoesNotExist_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _seatRepository.Delete(100));
            Assert.DoesNotThrowAsync(async () => await _seatRepository.SaveChanges());
        }

        private EquivalencyAssertionOptions<EventSeat> ExcludeProperties(EquivalencyAssertionOptions<EventSeat> options)
        {
            options.Excluding(o => o.Id);
            options.Excluding(o => o.Event);
            options.Excluding(o => o.Seat);
            return options;
        }

        private EquivalencyAssertionOptions<Seat> ExcludeProperties(EquivalencyAssertionOptions<Seat> options)
        {
            options.Excluding(o => o.Id);
            options.Excluding(o => o.Venue);
            options.Excluding(o => o.Section);
            return options;
        }
    }
}
