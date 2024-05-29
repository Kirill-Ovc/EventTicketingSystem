using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IBookingSeatRepository
{
    Task<ICollection<BookingSeat>> GetSeats(int bookingId);
    Task AddSeat(int bookingId, int eventSeatId, int offerId);
    Task DeleteSeat(int bookingId, int eventSeatId);
    Task<BookingSeat> GetSeat(int eventSeatId);
    Task BookSeats(int bookingId);
    Task<decimal> GetTotalPrice(int bookingId);
}