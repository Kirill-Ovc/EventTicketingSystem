using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.API.Interfaces
{
    public interface IBookingCartMapper
    {
        Task<Cart> MapBookingToCart(Booking booking);
    }
}