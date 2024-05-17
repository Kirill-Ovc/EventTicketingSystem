using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly DatabaseContext _context;

        public PaymentRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Payment>> GetPaymentsByBooking(int bookingId)
        {
            return await _context.Payments.Where(p => p.BookingId == bookingId).ToListAsync();
        }

        public async Task<ICollection<Payment>> GetPaymentsByUser(int userId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Where(p => p.Booking.UserId == userId)
                .ToListAsync();
        }

        public async Task<ICollection<Payment>> GetPendingPayments()
        {
            return await _context.Payments.Where(p => p.PaymentStatus == PaymentStatus.Pending).ToListAsync();
        }
    }
}
