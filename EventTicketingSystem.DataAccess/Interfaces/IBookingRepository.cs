using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface IBookingRepository: IRepository<Booking>
{
    Task<ICollection<Booking>> GetByUserId(int userId);
    Task<ICollection<Booking>> GetActiveBookings();
}