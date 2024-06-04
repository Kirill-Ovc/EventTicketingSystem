using EventTicketingSystem.API.Exceptions;
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
        private readonly IOfferRepository _offerRepository;
        private static readonly int _expirationTimeInMinutes = 10;

        public OrderService(ILogger<OrderService> logger,
            IBookingRepository bookingRepository,
            IBookingSeatRepository bookingSeatRepository,
            IPaymentRepository paymentRepository,
            IOfferRepository offerRepository)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _bookingSeatRepository = bookingSeatRepository;
            _paymentRepository = paymentRepository;
            _offerRepository = offerRepository;
        }

        public async Task<Cart> GetCart(string cartId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking is null)
            {
                _logger.LogWarning("Booking not found. CartId = {CartId}", cartId);
                throw new EntityNotFoundException("Cart not found");
            }

            var cart = MapBookingToCart(booking);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<Cart> AddToCart(string cartId, SeatOrder order)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking is null)
            {
                _logger.LogWarning("Booking not found and will be created. CartId = {CartId}", cartId);
                booking = await CreateBooking(order.UserId, cartId);
            }

            var bookingSeat = await _bookingSeatRepository.GetSeat(order.EventSeatId);
            if (bookingSeat is not null && bookingSeat.BookingId != booking.Id)
            {
                throw new BusinessException("Seat is already booked");
            }

            await AddSeat(booking.Id, order.EventSeatId, order.OfferId);

            var cart = MapBookingToCart(booking);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<Cart> RemoveFromCart(string cartId, int eventSeatId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking is null)
            {
                _logger.LogWarning("Booking not found. CartId = {CartId}", cartId);
                throw new EntityNotFoundException("Cart not found");
            }

            var bookingSeat = await _bookingSeatRepository.GetSeat(eventSeatId);
            if (bookingSeat is null || bookingSeat.BookingId != booking.Id)
            {
                _logger.LogInformation("Seat is not in the cart");
            }

            await _bookingSeatRepository.DeleteSeat(booking.Id, eventSeatId);

            var cart = MapBookingToCart(booking);
            cart.CartItems = await GetCartItems(booking.Id);

            return cart;
        }

        public async Task<int> CheckoutCart(string cartId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking is null)
            {
                _logger.LogWarning("Booking not found. CartId = {CartId}", cartId);
                throw new EntityNotFoundException("Cart not found");
            }

            booking.Status = BookingStatus.Active;
            booking.Price = await _bookingSeatRepository.GetTotalPrice(booking.Id);
            await _bookingSeatRepository.UpdateSeatsStatus(booking.Id, EventSeatStatus.Booked);

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
                Status = booking.Status.ToString(),
                ExpirationTimeStamp = booking.ExpirationTimeStamp,
                CartItems = new List<CartItem>()
            };
        }

        public async Task AddSeat(int bookingId, int eventSeatId, int offerId)
        {
            var offer = await _offerRepository.Find(offerId);
            if (offer == null)
            {
                throw new BusinessException($"Price offer not found. OfferId = {offerId}");
            }
            var bookingSeat = new BookingSeat()
            {
                BookingId = bookingId,
                EventSeatId = eventSeatId,
                Price = offer.Price,
                TicketLevel = offer.TicketLevel
            };

            await _bookingSeatRepository.Add(bookingSeat);
        }
    }
}
