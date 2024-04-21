using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class BookingRepository : IBookingRepository
    {
        private readonly DatabaseContext _context;

        public BookingRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Add(Booking booking)
        {
            await _context.AddAsync(booking);
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }
        
        public async Task<ICollection<Booking>> GetByUserId(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId && 
                            b.Status == BookingStatus.Active && 
                            b.ExpirationTimeStamp > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<ICollection<Booking>> GetActiveBookings()
        {
            return await _context.Bookings
                .Where(b => b.Status == BookingStatus.Active && 
                            b.ExpirationTimeStamp > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
        }
    }
}
