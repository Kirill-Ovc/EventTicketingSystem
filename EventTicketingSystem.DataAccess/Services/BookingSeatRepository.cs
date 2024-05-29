using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class BookingSeatRepository : IBookingSeatRepository
    {
        private readonly DatabaseContext _context;

        public BookingSeatRepository(DatabaseContext context)
        {
            _context = context;
        }
        
        public async Task<ICollection<BookingSeat>> GetSeats(int bookingId)
        {
            return await _context.BookingSeats
                .Include(bs => bs.EventSeat)
                .ThenInclude(es => es.Event)
                .Where(bs => bs.BookingId == bookingId)
                .ToListAsync();
        }

        public async Task AddSeat(int bookingId, int eventSeatId, int offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
            {
                throw new ArgumentException($"Price offer not found. OfferId = {offerId}");
            }
            var bookingSeat = new BookingSeat()
            {
                BookingId = bookingId,
                EventSeatId = eventSeatId,
                Price = offer.Price,
                TicketLevel = offer.TicketLevel
            };

            await _context.BookingSeats.AddAsync(bookingSeat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSeat(int bookingId, int eventSeatId)
        {
            var seat = await _context.BookingSeats
                .FirstOrDefaultAsync(bs => bs.BookingId == bookingId && bs.EventSeatId == eventSeatId);

            if (seat != null)
            {
                _context.BookingSeats.Remove(seat);
            }
            await _context.SaveChangesAsync();
        }

        public Task<BookingSeat> GetSeat(int eventSeatId)
        {
            return _context.BookingSeats.FirstOrDefaultAsync(bs => bs.EventSeatId == eventSeatId);
        }

        public async Task UpdateSeatsStatus(int bookingId, EventSeatStatus status)
        {
            var seats = await _context.BookingSeats
                .Include(bs => bs.EventSeat)
                .Where(bs => bs.BookingId == bookingId)
                .Select(bs => bs.EventSeat)
                .ToListAsync();

            foreach (var seat in seats)
            {
                seat.Status = status;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPrice(int bookingId)
        {
            return await _context.BookingSeats
                .Where(bs => bs.BookingId == bookingId)
                .SumAsync(bs => bs.Price);
        }
    }
}
