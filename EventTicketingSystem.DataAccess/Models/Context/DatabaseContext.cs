using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Models.Context
{
    public class DatabaseContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}
