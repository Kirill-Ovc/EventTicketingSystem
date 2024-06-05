using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using FluentAssertions;
using NSubstitute;

namespace EventTicketingSystem.Tests.Services
{
    [TestFixture]
    public class BookingSeatServiceTests
    {
        private BookingSeatService _service;
        private IBookingSeatRepository _bookingSeatRepository;
        private IOfferRepository _offerRepository;

        [SetUp]
        public void SetUp()
        {
            _bookingSeatRepository = Substitute.For<IBookingSeatRepository>();
            _offerRepository = Substitute.For<IOfferRepository>();
            _service = new BookingSeatService(_bookingSeatRepository, _offerRepository);
        }

        [Test]
        public async Task AddSeat_SeatBooked_ThrowsException()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            var offerId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns(new BookingSeat());

            // Act
            Func<Task> action = async () => await _service.AddSeat(bookingId, eventSeatId, offerId);

            // Assert
            await action.Should().ThrowAsync<BusinessException>().WithMessage("Seat is already booked");
            await _bookingSeatRepository.Received(1).GetSeat(eventSeatId);
        }

        [Test]
        public async Task AddSeat_OfferNotFound_ThrowsException()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            var offerId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns((BookingSeat)null);
            _offerRepository.Find(offerId).Returns((Offer)null);

            // Act
            Func<Task> action = async () => await _service.AddSeat(bookingId, eventSeatId, offerId);

            // Assert
            await action.Should().ThrowAsync<BusinessException>().WithMessage("Price offer not found. OfferId = 1");
            await _offerRepository.Received(1).Find(offerId);
        }

        [Test]
        public async Task AddSeat_ValidOffer_AddsSeat()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            var offerId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns((BookingSeat)null);
            _offerRepository.Find(offerId).Returns(new Offer { Price = 100, TicketLevel = TicketLevel.Vip });

            // Act
            await _service.AddSeat(bookingId, eventSeatId, offerId);

            // Assert
            await _bookingSeatRepository.Received(1).Add(Arg.Is<BookingSeat>(x => 
                x.BookingId == bookingId && 
                x.EventSeatId == eventSeatId && 
                x.Price == 100 && 
                x.TicketLevel == TicketLevel.Vip));
        }

        [Test]
        public async Task RemoveSeat_SeatNotBooked_ThrowsException()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns((BookingSeat)null);

            // Act
            Func<Task> action = async () => await _service.RemoveSeat(bookingId, eventSeatId);

            // Assert
            await action.Should().ThrowAsync<BusinessException>().WithMessage("Seat is not booked");
            await _bookingSeatRepository.Received(1).GetSeat(eventSeatId);
        }

        [Test]
        public async Task RemoveSeat_SeatNotInCart_ThrowsException()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns(new BookingSeat { BookingId = 2 });

            // Act
            Func<Task> action = async () => await _service.RemoveSeat(bookingId, eventSeatId);

            // Assert
            await action.Should().ThrowAsync<BusinessException>().WithMessage("Seat is not in the cart");
            await _bookingSeatRepository.Received(1).GetSeat(eventSeatId);
        }

        [Test]
        public async Task RemoveSeat_ValidSeat_RemovesSeat()
        {
            // Arrange
            var bookingId = 1;
            var eventSeatId = 1;
            _bookingSeatRepository.GetSeat(eventSeatId).Returns(new BookingSeat { BookingId = bookingId });

            // Act
            await _service.RemoveSeat(bookingId, eventSeatId);

            // Assert
            await _bookingSeatRepository.Received(1).DeleteSeat(bookingId, eventSeatId);
        }

        [Test]
        public async Task BookSeats_ValidSeats_UpdatesStatus()
        {
            // Arrange
            var bookingId = 1;

            // Act
            await _service.BookSeats(bookingId);

            // Assert
            await _bookingSeatRepository.Received(1).UpdateSeatsStatus(bookingId, EventSeatStatus.Booked);
        }
    }
}
