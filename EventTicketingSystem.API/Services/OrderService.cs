﻿using EventTicketingSystem.API.Constants;
using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace EventTicketingSystem.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingCartMapper _bookingCartMapper;
        private readonly IBookingSeatService _bookingSeatService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMemoryCache _cache;
        private static readonly int _expirationTimeInMinutes = 10;

        public OrderService(ILogger<OrderService> logger,
            IBookingRepository bookingRepository,
            IBookingCartMapper bookingCartMapper,
            IBookingSeatService bookingSeatService,
            IPaymentRepository paymentRepository,
            IMemoryCache cache)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
            _bookingCartMapper = bookingCartMapper;
            _bookingSeatService = bookingSeatService;
            _paymentRepository = paymentRepository;
            _cache = cache;
        }

        public async Task<Cart> GetCart(string cartId)
        {
            var booking = await _bookingRepository.GetByUuid(cartId);
            if (booking is null)
            {
                _logger.LogWarning("Booking not found. CartId = {CartId}", cartId);
                throw new EntityNotFoundException("Cart not found");
            }

            var cart = await _bookingCartMapper.MapBookingToCart(booking);

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

            await _bookingSeatService.AddSeat(booking.Id, order.EventSeatId, order.OfferId);

            InvalidateEventCache(order.EventId);

            var cart = await _bookingCartMapper.MapBookingToCart(booking);

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

            await _bookingSeatService.RemoveSeat(booking.Id, eventSeatId);

            var cart = await _bookingCartMapper.MapBookingToCart(booking);
            foreach (var seat in cart.CartItems)
            {
                InvalidateEventCache(seat.EventId);
            }

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
            booking.Price = await _bookingRepository.CalculateTotalPrice(booking.Id);
            await _bookingSeatService.BookSeats(booking.Id);
            await InvalidateEventCache(booking);

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

        private void InvalidateEventCache(int eventId)
        {
            var cacheKey = string.Format(CacheKeys.EventSeats, eventId);
            _cache.Remove(cacheKey);
        }

        private async Task InvalidateEventCache(Booking booking)
        {
            var cart = await _bookingCartMapper.MapBookingToCart(booking);
            foreach (var seat in cart.CartItems)
            {
                InvalidateEventCache(seat.EventId);
            }
        }
    }
}
