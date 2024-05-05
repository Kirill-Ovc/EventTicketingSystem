using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Models.Context
{
    internal class DatabaseContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }
        
        public DbSet<Venue> Venues { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventInfo> EventInfos { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<EventSeat> EventSeats { get; set; }

        public DbSet<Offer> Offers { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}
