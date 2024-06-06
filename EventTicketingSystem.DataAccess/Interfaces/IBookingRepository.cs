using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<ICollection<Booking>> GetByUserId(int userId);
    Task<ICollection<Booking>> GetActiveBookings();
    Task<Booking> GetByUuid(string uuid);
    Task<decimal> CalculateTotalPrice(int bookingId);
}