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
    public class BookingRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IBookingRepository _bookingRepository;
        private DatabaseContext _context;

        public BookingRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _bookingRepository = new BookingRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedCities();
            await seeder.SeedVenues();
            await seeder.SeedSeats(1);
            await seeder.SeedEvents();
            await seeder.SeedBookings();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task BookingRepository_GetByUserId_ReturnsBookings()
        {
            var expectedBooking = new Booking
            {
                UserId = 1,
                EventSeatId = 21,
                Status = BookingStatus.Active,
                ExpirationTimeStamp = DateTime.Today.AddDays(1),
                Price = 1000
            };

            var bookings = await _bookingRepository.GetByUserId(1);

            bookings.Should().ContainSingle();
            bookings.First().Should().BeEquivalentTo(expectedBooking, ExcludeProperties);
        }

        [Test]
        public async Task BookingRepository_GetActiveBookings_ReturnsActiveBookings()
        {
            var expectedBookings = _dataProvider.GetBookings()
                .Where(b => b.Status == BookingStatus.Active)
                .ToList();

            var bookings = await _bookingRepository.GetActiveBookings();

            bookings.Should().BeEquivalentTo(expectedBookings, ExcludeProperties);
        }

        private EquivalencyAssertionOptions<Booking> ExcludeProperties(EquivalencyAssertionOptions<Booking> options)
        {
            options
                .Excluding(o => o.Id)
                .Excluding(o => o.User)
                .Excluding(o => o.EventSeat);
            return options;
        }
    }
}
