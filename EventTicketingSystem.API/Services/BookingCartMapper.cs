using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.API.Services
{
    public class BookingCartMapper : IBookingCartMapper
    {
        private readonly IBookingSeatRepository _bookingSeatRepository;

        public BookingCartMapper(IBookingSeatRepository bookingSeatRepository)
        {
            _bookingSeatRepository = bookingSeatRepository;
        }

        public async Task<Cart> MapBookingToCart(Booking booking)
        {
            return new Cart()
            {
                CartId = booking.Uuid,
                UserId = booking.UserId,
                Status = booking.Status.ToString(),
                ExpirationTimeStamp = booking.ExpirationTimeStamp,
                CartItems = await GetCartItems(booking.Id)
            };
        }

        private async Task<List<CartItem>> GetCartItems(int bookingId)
        {
            var cartItems = await _bookingSeatRepository.GetSeats(bookingId);
            return cartItems.Select(ci => new CartItem()
            {
                EventSeatId = ci.EventSeatId,
                Price = ci.Price,
                EventId = ci.EventSeat.EventId,
                EventName = ci.EventSeat.Event.Name
            }).ToList();
        }
    }
}
