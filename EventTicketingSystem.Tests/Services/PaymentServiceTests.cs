using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using FluentAssertions;
using NSubstitute;

namespace EventTicketingSystem.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private PaymentService _paymentService;
        private IPaymentRepository _paymentRepository;
        private IBookingRepository _bookingRepository;
        private IBookingSeatRepository _bookingSeatRepository;
        private INotificationService _notificationService;
        private Payment _foundPayment;
        private Booking _foundBooking;

        [SetUp]
        public void SetUp()
        {
            _paymentRepository = Substitute.For<IPaymentRepository>();
            _bookingRepository = Substitute.For<IBookingRepository>();
            _bookingSeatRepository = Substitute.For<IBookingSeatRepository>();
            _notificationService = Substitute.For<INotificationService>();
            _paymentService = new PaymentService(_paymentRepository, _bookingRepository, _bookingSeatRepository, _notificationService);
            _foundPayment = new Payment
            {
                Id = 1,
                PaymentStatus = PaymentStatus.Pending,
                BookingId = 5,
                Amount = 100
            };
            _foundBooking = new Booking
            {
                Id = 5,
                Price = 100,
                UserId = 1,
                Status = BookingStatus.Active
            };
        }

        [Test]
        public async Task GetPaymentStatus_WithExistingPayment_ReturnsStatus()
        {
            // Arrange
            var paymentId = 1;
            var paymentStatus = PaymentStatus.Pending;
            _paymentRepository.Find(paymentId).Returns(new Payment { PaymentStatus = paymentStatus });

            // Act
            var result = await _paymentService.GetPaymentStatus(paymentId);

            // Assert
            result.Should().Be(paymentStatus);
            await _paymentRepository.Received(1).Find(paymentId);
        }

        [Test]
        public async Task GetPaymentStatus_WithNonExistingPayment_ThrowsException()
        {
            // Arrange
            var paymentId = 1;
            _paymentRepository.Find(paymentId).Returns((Payment)null);

            // Act
            Func<Task> act = async () => await _paymentService.GetPaymentStatus(paymentId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Payment not found");
            await _paymentRepository.Received(1).Find(paymentId);
        }

        [Test]
        public async Task CompletePayment_WithValidPaymentId_ReturnsOk()
        {
            // Arrange
            var payment = _foundPayment;
            var booking = _foundBooking;
            var paymentStatus = PaymentStatus.Paid;
            _paymentRepository.Find(payment.Id).Returns(payment);
            _bookingRepository.Find(booking.Id).Returns(booking);
            
            // Act
            var result = await _paymentService.PaymentCompleted(payment.Id);

            // Assert
            result.Should().Be(paymentStatus);
            await _paymentRepository.Received(1).Find(payment.Id);
            await _paymentRepository.Received(1).Update(Arg.Is<Payment>(p => p.PaymentStatus == PaymentStatus.Paid));
            await _bookingRepository.Received(1).Find(booking.Id);
            await _bookingRepository.Received(1).Update(Arg.Is<Booking>(b => b.Status == BookingStatus.Paid));
            await _bookingSeatRepository.Received(1).UpdateSeatsStatus(booking.Id, EventSeatStatus.Sold);
        }

        [Test]
        public async Task CompletePayment_WithInvalidPaymentId_ThrowsException()
        {
            // Arrange
            var paymentId = 1;
            _paymentRepository.Find(paymentId).Returns((Payment)null);

            // Act
            Func<Task> act = async () => await _paymentService.PaymentCompleted(paymentId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Payment not found");
            await _paymentRepository.Received(1).Find(paymentId);
            await _paymentRepository.DidNotReceive().Update(Arg.Any<Payment>());
        }

        [Test]
        public async Task CompletePayment_WithInvalidBookingId_ThrowsException()
        {
            // Arrange
            var payment = _foundPayment;
            var expectedMessage = "No active booking found for this payment";
            _paymentRepository.Find(payment.Id).Returns(payment);
            _bookingRepository.Find(payment.BookingId).Returns((Booking)null);

            // Act
            Func<Task> act = async () => await _paymentService.PaymentCompleted(payment.Id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(expectedMessage);
            await _paymentRepository.Received(1).Find(payment.Id);
            await _paymentRepository.Received(1).Update(Arg.Any<Payment>());
            await _bookingRepository.Received(1).Find(payment.BookingId);
            await _bookingRepository.DidNotReceive().Update(Arg.Any<Booking>());
        }

        [Test]
        public async Task CompletePayment_WithWrongBookingPrice_ThrowsException()
        {
            // Arrange
            var payment = _foundPayment;
            var booking = new Booking { Id = payment.BookingId, Price = 200, Status = BookingStatus.Active };
            _paymentRepository.Find(payment.Id).Returns(payment);
            _bookingRepository.Find(booking.Id).Returns(booking);

            // Act
            Func<Task> act = async () => await _paymentService.PaymentCompleted(payment.Id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Invalid booking price");
            await _paymentRepository.Received(1).Find(payment.Id);
            await _paymentRepository.Received(1).Update(Arg.Any<Payment>());
            await _bookingRepository.Received(1).Find(booking.Id);
            await _bookingRepository.DidNotReceive().Update(Arg.Any<Booking>());
        }

        [Test]
        public async Task PaymentFailed_WithValidPaymentId_ReturnsOk()
        {
            // Arrange
            var payment = _foundPayment;
            var booking = _foundBooking;
            var paymentStatus = PaymentStatus.Cancelled;
            _paymentRepository.Find(payment.Id).Returns(payment);
            _bookingRepository.Find(booking.Id).Returns(booking);

            // Act
            var result = await _paymentService.PaymentFailed(payment.Id);

            // Assert
            result.Should().Be(paymentStatus);
            await _paymentRepository.Received(1).Find(payment.Id);
            await _paymentRepository.Received(1).Update(Arg.Is<Payment>(p => p.PaymentStatus == PaymentStatus.Cancelled));
            await _bookingRepository.Received(1).Find(booking.Id);
            await _bookingRepository.Received(1).Update(Arg.Is<Booking>(b => b.Status == BookingStatus.Cancelled));
            await _bookingSeatRepository.Received(1).UpdateSeatsStatus(booking.Id, EventSeatStatus.Available);
        }

        [Test]
        public async Task PaymentFailed_WithInvalidPaymentId_ThrowsException()
        {
            // Arrange
            var paymentId = 1;
            _paymentRepository.Find(paymentId).Returns((Payment)null);

            // Act
            Func<Task> act = async () => await _paymentService.PaymentFailed(paymentId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Payment not found");
            await _paymentRepository.Received(1).Find(paymentId);
            await _paymentRepository.DidNotReceive().Update(Arg.Any<Payment>());
        }
    }
}
