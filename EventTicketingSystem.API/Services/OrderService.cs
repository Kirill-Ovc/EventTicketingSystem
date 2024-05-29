using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingSeatRepository _bookingSeatRepository;
        private readonly IPaymentRepository _paymentRepository;
        private static readonly int _expirationTimeInMinutes = 10;

        public OrderService(ILogger<OrderService> logger,
            IBookingRepository bookingRepository,
            IBookingSeatRepository bookingSeatRepository,
            IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _bookingSeatRepository = bookingSeatRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<Cart> GetCart(string cartId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking == null)
            {
                _logger.LogWarning("Cart not found. CartId = {CartId}", cartId);
                return null;
            }

            var cart = MapBookingToCart(booking);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<Cart> AddToCart(string cartId, SeatOrder order)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking == null)
            {
                booking = await CreateBooking(order.UserId, cartId);
            }

            var cart = MapBookingToCart(booking);

            var bookingSeat = await _bookingSeatRepository.GetSeat(order.EventSeatId);
            if (bookingSeat != null && bookingSeat.BookingId != booking.Id)
            {
                throw new InvalidOperationException("Seat is already booked");
            }

            await _bookingSeatRepository.AddSeat(booking.Id, order.EventSeatId, order.OfferId);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<Cart> RemoveFromCart(string cartId, int eventSeatId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking == null)
            {
                _logger.LogWarning("Cart not found. CartId = {CartId}", cartId);
                return null;
            }

            var cart = MapBookingToCart(booking);

            var bookingSeat = await _bookingSeatRepository.GetSeat(eventSeatId);
            if (bookingSeat == null || bookingSeat.BookingId != booking.Id)
            {
                _logger.LogInformation("Seat is not in the cart");
            }

            await _bookingSeatRepository.DeleteSeat(booking.Id, eventSeatId);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<int> CheckoutCart(string cartId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking == null)
            {
                _logger.LogWarning("Cart not found. CartId = {CartId}", cartId);
                throw new InvalidOperationException("Cart not found");
            }

            booking.Status = BookingStatus.Active;
            booking.Price = await _bookingSeatRepository.GetTotalPrice(booking.Id);
            await _bookingSeatRepository.BookSeats(booking.Id);

            var newPayment = new Payment()
            {
                BookingId = booking.Id,
                Amount = booking.Price
            };
            await _paymentRepository.Add(newPayment);
            await _paymentRepository.SaveChanges();

            return newPayment.Id;
        }

        private async Task<Booking> CreateBooking(int userId, string cartId)
        {
            var booking = new Booking()
            {
                UserId = userId,
                Uuid = cartId,
                Status = BookingStatus.Active,
                ExpirationTimeStamp = DateTime.UtcNow.AddMinutes(_expirationTimeInMinutes)
            };

            await _bookingRepository.Add(booking);
            await _bookingRepository.SaveChanges();
            return booking;
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

        private Cart MapBookingToCart(Booking booking)
        {
            return new Cart()
            {
                CartId = booking.Uuid,
                UserId = booking.UserId,
                Status = booking.Status,
                ExpirationTimeStamp = booking.ExpirationTimeStamp,
                CartItems = new List<CartItem>()
            };
        }
    }
}
