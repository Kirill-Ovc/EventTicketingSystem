using EventTicketingSystem.API.Models;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using FluentAssertions;
using NSubstitute;

namespace EventTicketingSystem.Tests.Services
{
    [TestFixture]
    public class BookingCartMapperTests
    {
        private BookingCartMapper _service;
        private IBookingSeatRepository _bookingSeatRepository;

        [SetUp]
        public void SetUp()
        {
            _bookingSeatRepository = Substitute.For<IBookingSeatRepository>();
            _service = new BookingCartMapper(_bookingSeatRepository);
        }

        [Test]
        public async Task MapBookingToCart_WithBooking_ReturnsCart()
        {
            // Arrange
            _bookingSeatRepository.GetSeats(_booking.Id).Returns(_bookingSeats);

            // Act
            var result = await _service.MapBookingToCart(_booking);

            // Assert
            result.Should().BeEquivalentTo(_cart);
        }

        #region TestData
        private readonly Booking _booking = new Booking
        {
            Id = 111,
            Uuid = "cartId",
            UserId = 1,
            Status = BookingStatus.Active,
            ExpirationTimeStamp = DateTime.Today.AddDays(1)
        };
        private readonly Cart _cart = new Cart
        {
            CartId = "cartId",
            UserId = 1,
            Status = "Active",
            ExpirationTimeStamp = DateTime.Today.AddDays(1),
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    EventId = 1,
                    EventSeatId = 1,
                    EventName = "Event 1",
                    Price = 10
                },
                new CartItem
                {
                    EventId = 1,
                    EventSeatId = 2,
                    EventName = "Event 1",
                    Price = 20
                }
            }
        };
        private static readonly EventSeat _eventSeatInfo =
            new EventSeat { EventId = 1, Event = new Event { Name = "Event 1" } };
        private static readonly List<BookingSeat> _bookingSeats = new List<BookingSeat>
        {
            new() { EventSeatId = 1, Price = 10, EventSeat = _eventSeatInfo },
            new() { EventSeatId = 2, Price = 20, EventSeat = _eventSeatInfo }
        };
        #endregion
    }
}
