using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingSeatRepository _bookingSeatRepository;

        public PaymentService(IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IBookingSeatRepository bookingSeatRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _bookingSeatRepository = bookingSeatRepository;
        }

        public async Task<PaymentStatus> GetPaymentStatus(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }
            return payment.PaymentStatus;
        }

        public async Task<PaymentStatus> PaymentCompleted(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            if (payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                await _paymentRepository.Update(payment);
                await _paymentRepository.SaveChanges();
            }

            var booking = await _bookingRepository.Find(payment.BookingId);
            if (booking is { Status: BookingStatus.Active } && booking.Price == payment.Amount)
            {
                booking.Status = BookingStatus.Paid;
                await _bookingRepository.Update(booking);
                await _bookingSeatRepository.UpdateSeatsStatus(booking.Id, EventSeatStatus.Sold);
            }

            return payment.PaymentStatus;
        }

        public async Task<PaymentStatus> PaymentFailed(int paymentId)
        {
            var payment = await _paymentRepository.Find(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            if (payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Cancelled;
                await _paymentRepository.Update(payment);
                await _paymentRepository.SaveChanges();
            }

            var booking = await _bookingRepository.Find(payment.BookingId);
            if (booking is { Status: BookingStatus.Active } && booking.Price == payment.Amount)
            {
                booking.Status = BookingStatus.Paid;
                await _bookingRepository.Update(booking);
                await _bookingSeatRepository.UpdateSeatsStatus(booking.Id, EventSeatStatus.Sold);
            }

            return payment.PaymentStatus;
        }

    }
}
