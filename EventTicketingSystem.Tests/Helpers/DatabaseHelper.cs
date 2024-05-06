using EventTicketingSystem.DataAccess.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Tests.Helpers
{
    internal static class DatabaseHelper
    {
        public static DatabaseContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
