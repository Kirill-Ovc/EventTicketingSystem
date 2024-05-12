using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly DatabaseContext _context;

        public BookingRepository(DatabaseContext context) : base(context)
        {
            _context = context;
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
    }
}
