using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<ICollection<Payment>> GetPaymentsByBooking(int bookingId);
    Task<ICollection<Payment>> GetPaymentsByUser(int userId);
    Task<ICollection<Payment>> GetPendingPayments();

}