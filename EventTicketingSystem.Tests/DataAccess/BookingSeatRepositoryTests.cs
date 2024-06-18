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
    public class BookingSeatRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IBookingSeatRepository _seatRepository;
        private DatabaseContext _context;
        private TestDataSeeder _seeder;
        private const int BookingId = 5;

        public BookingSeatRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _seatRepository = new BookingSeatRepository(_context);
            _seeder = new TestDataSeeder(_context, _dataProvider);
            await _seeder.SeedCities();
            await _seeder.SeedVenues();
            await _seeder.SeedSeats(1);
            await _seeder.SeedEvents();
            await _seeder.SeedEventSeats(1, 1);
            await _seeder.SeedBookingSeats(BookingId);
        }

        [Test]
        public async Task GetSeats_ReturnsSeats()
        {
            // Arrange
            var expectedSeat = new BookingSeat()
            {
                Id = 1,
                BookingId = BookingId,
                EventSeatId = 1,
                TicketLevel = TicketLevel.Other,
                Price = 200
            };

            // Act
            var seats = await _seatRepository.GetSeats(BookingId);

            // Assert
            seats.Count.Should().Be(10);
            var seat = seats.OrderBy(x => x.Id).First();
            seat.Should().BeEquivalentTo(expectedSeat, opt => 
                opt.Excluding(x => x.EventSeat));
            seat.EventSeat.Should().NotBeNull();
        }

        [Test]
        public async Task AddSeat_GetSeat_ReturnsSeat()
        {
            // Arrange
            var expectedSeat = new BookingSeat()
            {
                Id = 101,
                BookingId = BookingId,
                EventSeatId = 101,
                TicketLevel = TicketLevel.Other,
                Price = 200
            };

            // Act
            await _seatRepository.Add(expectedSeat);
            var seat = await _seatRepository.GetSeat(expectedSeat.EventSeatId);

            // Assert
            seat.Should().BeEquivalentTo(expectedSeat, opt => 
                               opt.Excluding(x => x.EventSeat));
        }

        [Test]
        public async Task DeleteSeat_ReturnsNull()
        {
            // Arrange
            var eventSeatId = 10;
            var seat = await _seatRepository.GetSeat(eventSeatId);
            seat.Should().NotBeNull();

            // Act
            await _seatRepository.DeleteSeat(BookingId, eventSeatId);
            
            // Assert
            seat = await _seatRepository.GetSeat(eventSeatId);
            seat.Should().BeNull();
        }

        [Test]
        public async Task UpdateSeatsStatus_ReturnsSeats()
        {
            // Arrange
            var status = EventSeatStatus.Booked;

            // Act
            await _seatRepository.UpdateSeatsStatus(BookingId, status);

            // Assert
            var seats = await _seatRepository.GetSeats(BookingId);
            seats.All(x => x.EventSeat.Status == status).Should().BeTrue();
        }
    }
}
