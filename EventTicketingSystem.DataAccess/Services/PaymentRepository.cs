using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _context;

        public PaymentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Payment> Find(int id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task Add(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public Task Update(Payment payment)
        {
            _context.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public async Task Delete(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                payment.PaymentStatus = PaymentStatus.Cancelled;
            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
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
