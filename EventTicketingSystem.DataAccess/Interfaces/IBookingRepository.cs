using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface IBookingRepository
{
    Task Add(Booking booking);
    void Update(Booking booking);
    Task<ICollection<Booking>> GetByUserId(int userId);
    Task<ICollection<Booking>> GetActiveBookings();
    Task Delete(int id);
}