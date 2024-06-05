using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Services
{
    public class BookingSeatService : IBookingSeatService
    {
        private readonly IBookingSeatRepository _bookingSeatRepository;
        private readonly IOfferRepository _offerRepository;

        public BookingSeatService(
            IBookingSeatRepository bookingSeatRepository,
            IOfferRepository offerRepository)
        {
            _bookingSeatRepository = bookingSeatRepository;
            _offerRepository = offerRepository;
        }

        public async Task AddSeat(int bookingId, int eventSeatId, int offerId)
        {
            var bookingSeat = await _bookingSeatRepository.GetSeat(eventSeatId);
            if (bookingSeat is not null)
            {
                throw new BusinessException("Seat is already booked");
            }

            var offer = await _offerRepository.Find(offerId);
            if (offer == null)
            {
                throw new BusinessException($"Price offer not found. OfferId = {offerId}");
            }

            bookingSeat = new BookingSeat()
            {
                BookingId = bookingId,
                EventSeatId = eventSeatId,
                Price = offer.Price,
                TicketLevel = offer.TicketLevel
            };

            await _bookingSeatRepository.Add(bookingSeat);
        }

        public async Task RemoveSeat(int bookingId, int eventSeatId)
        {
            var bookingSeat = await _bookingSeatRepository.GetSeat(eventSeatId);
            if (bookingSeat is null)
            {
                throw new BusinessException("Seat is not booked");
            }

            if (bookingSeat.BookingId != bookingId)
            {
                throw new BusinessException("Seat is not in the cart");
            }

            await _bookingSeatRepository.DeleteSeat(bookingId, eventSeatId);
        }

        public async Task BookSeats(int bookingId)
        {
            await _bookingSeatRepository.UpdateSeatsStatus(bookingId, EventSeatStatus.Booked);
        }
    }
}
