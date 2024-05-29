using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IBookingSeatRepository
{
    Task<ICollection<BookingSeat>> GetSeats(int bookingId);
    Task AddSeat(int bookingId, int eventSeatId, int offerId);
    Task DeleteSeat(int bookingId, int eventSeatId);
    Task<BookingSeat> GetSeat(int eventSeatId);
    Task UpdateSeatsStatus(int bookingId, EventSeatStatus status);
    Task<decimal> GetTotalPrice(int bookingId);
}