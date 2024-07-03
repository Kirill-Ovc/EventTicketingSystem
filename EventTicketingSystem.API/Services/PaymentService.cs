using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingSeatRepository _bookingSeatRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IBookingSeatRepository bookingSeatRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _bookingSeatRepository = bookingSeatRepository;
            _notificationService = notificationService;
        }

        public async Task<PaymentStatus> GetPaymentStatus(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment is null)
            {
                throw new EntityNotFoundException("Payment not found");
            }

            return payment.PaymentStatus;
        }

        public async Task<PaymentStatus> PaymentCompleted(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment is null)
            {
                throw new EntityNotFoundException("Payment not found");
            }

            if (payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;
                await _paymentRepository.Update(payment);
                await _paymentRepository.SaveChanges();
            }

            await UpdateBookingStatus(payment, BookingStatus.Paid, EventSeatStatus.Sold);
            await _notificationService.NotifyCheckoutCompletedAsync(payment.BookingId);

            return payment.PaymentStatus;
        }

        public async Task<PaymentStatus> PaymentFailed(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment is null)
            {
                throw new EntityNotFoundException("Payment not found");
            }

            if (payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Cancelled;
                await _paymentRepository.Update(payment);
                await _paymentRepository.SaveChanges();
            }

            await UpdateBookingStatus(payment, BookingStatus.Cancelled, EventSeatStatus.Available);

            return payment.PaymentStatus;
        }

        private async Task UpdateBookingStatus(Payment payment, BookingStatus status, EventSeatStatus seatStatus)
        {
            var booking = await _bookingRepository.Find(payment.BookingId);
            switch (booking)
            {
                case { Status: BookingStatus.Active } when booking.Price == payment.Amount:
                    booking.Status = status;
                    await _bookingRepository.Update(booking);
                    await _bookingSeatRepository.UpdateSeatsStatus(booking.Id, seatStatus);
                    break;
                case { Status: BookingStatus.Active } when booking.Price != payment.Amount:
                    throw new BusinessException("Invalid booking price");
                default:
                {
                    throw new BusinessException("No active booking found for this payment");
                }
            }
        }

    }
}
