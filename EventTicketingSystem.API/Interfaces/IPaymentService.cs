using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentStatus> GetPaymentStatus(int paymentId);

        Task<PaymentStatus> PaymentCompleted(int paymentId);

        Task<PaymentStatus> PaymentFailed(int paymentId);
    }
}