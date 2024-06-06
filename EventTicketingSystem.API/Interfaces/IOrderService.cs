using EventTicketingSystem.API.Models;

namespace EventTicketingSystem.API.Interfaces
{
    public interface IOrderService
    {
        Task<Cart> GetCart(string cartId);

        Task<Cart> AddToCart(string cartId, SeatOrder order);

        Task<Cart> RemoveFromCart(string cartId, int eventSeatId);

        Task<int> CheckoutCart(string cartId);
    }
}
