using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using EventTicketingSystem.API.Interfaces;

namespace EventTicketingSystem.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _service;
        private IBookingRepository _bookingRepository;
        private IBookingCartMapper _bookingCartMapper;
        private IBookingSeatService _bookingSeatService;
        private IPaymentRepository _paymentRepository;

        [SetUp]
        public void Setup()
        {
            var logger = Substitute.For<ILogger<OrderService>>();
            _bookingRepository = Substitute.For<IBookingRepository>();
            _bookingCartMapper = Substitute.For<IBookingCartMapper>();
            _bookingSeatService = Substitute.For<IBookingSeatService>();
            _paymentRepository = Substitute.For<IPaymentRepository>();

            _service = new OrderService(logger, 
                _bookingRepository, _bookingCartMapper, _bookingSeatService, _paymentRepository);
        }

        [Test]
        public async Task OrderService_GetCart_ReturnsCart()
        {
            // Arrange
            var cartId = Guid.NewGuid().ToString();
            _bookingRepository.GetByUuid(cartId).Returns(_booking);
            _bookingCartMapper.MapBookingToCart(_booking).Returns(_cart);

            // Act
            var result = await _service.GetCart(cartId);

            // Assert
            result.Should().BeEquivalentTo(_cart);
        }

        [Test]
        public async Task OrderService_GetCart_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cartId = Guid.NewGuid().ToString();
            _bookingRepository.GetByUuid(cartId).Returns((Booking)null);

            // Act
            Func<Task> act = async () => await _service.GetCart(cartId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Test]
        public async Task OrderService_AddToCart_AddSeatInExistingBooking()
        {
            // Arrange
            var order = new SeatOrder { UserId = 1, EventSeatId = 1, OfferId = 1 };
            _bookingRepository.GetByUuid(_booking.Uuid).Returns(_booking);
            _bookingCartMapper.MapBookingToCart(_booking).Returns(_cart);

            // Act
            var result = await _service.AddToCart(_booking.Uuid, order);

            // Assert
            result.Should().BeEquivalentTo(_cart);
            await _bookingSeatService.Received(1).AddSeat(_booking.Id, order.EventSeatId, order.OfferId);
        }

        [Test]
        public async Task OrderService_AddToCart_CreateBookingAndAddSeat()
        {
            // Arrange
            var order = new SeatOrder { UserId = 1, EventSeatId = 1, OfferId = 1 };
            _bookingRepository.GetByUuid(_booking.Uuid).Returns((Booking)null);
            _bookingCartMapper.MapBookingToCart(_booking).Returns(_cart);

            // Act
            await _service.AddToCart(_booking.Uuid, order);

            // Assert
            await _bookingRepository.Received(1).Add(Arg.Is<Booking>(b => 
                b.Uuid == _booking.Uuid && b.UserId == order.UserId && b.Status == BookingStatus.Active));
            await _bookingSeatService.Received(1).AddSeat(Arg.Any<int>(), order.EventSeatId, order.OfferId);
        }

        [Test]
        public async Task OrderService_RemoveFromCart_RemoveSeatFromBooking()
        {
            // Arrange
            var eventSeatId = 3;
            _bookingRepository.GetByUuid(_booking.Uuid).Returns(_booking);
            _bookingCartMapper.MapBookingToCart(_booking).Returns(_cart);

            // Act
            var result = await _service.RemoveFromCart(_booking.Uuid, eventSeatId);

            // Assert
            result.Should().BeEquivalentTo(_cart);
            await _bookingSeatService.Received(1).RemoveSeat(_booking.Id, eventSeatId);
        }

        [Test]
        public async Task OrderService_RemoveFromCart_ThrowsEntityNotFoundException()
        {
            // Arrange
            var eventSeatId = 3;
            _bookingRepository.GetByUuid(_booking.Uuid).Returns((Booking)null);

            // Act
            Func<Task> act = async () => await _service.RemoveFromCart(_booking.Uuid, eventSeatId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Test]
        public async Task OrderService_CheckoutCart_ReturnsPaymentId()
        {
            // Arrange
            var paymentId = 456;
            _bookingRepository.GetByUuid(_booking.Uuid).Returns(_booking);
            _bookingRepository.CalculateTotalPrice(_booking.Id).Returns(100);
            _bookingCartMapper.MapBookingToCart(_booking).Returns(_cart);
            _paymentRepository
                .When(x => 
                    x.Add(Arg.Is<Payment>(p => 
                        p.BookingId == _booking.Id)))
                .Do(x =>
                {
                    var payment = x.Arg<Payment>();
                    payment.Id = paymentId;
                });

            _paymentRepository.SaveChanges().Returns(Task.CompletedTask);

            // Act
            var result = await _service.CheckoutCart(_booking.Uuid);

            // Assert
            result.Should().Be(paymentId);
            await _bookingSeatService.Received(1).BookSeats(_booking.Id);
            await _paymentRepository.Received(1).Add(Arg.Is<Payment>(p => p.BookingId == _booking.Id));
            await _paymentRepository.Received(1).SaveChanges();
        }

        [Test]
        public async Task OrderService_CheckoutCart_ThrowsEntityNotFoundException()
        {
            // Arrange
            _bookingRepository.GetByUuid(_booking.Uuid).Returns((Booking)null);

            // Act
            Func<Task> act = async () => await _service.CheckoutCart(_booking.Uuid);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
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
        #endregion
    }
}
