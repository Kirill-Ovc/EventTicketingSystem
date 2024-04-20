using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EventTicketingSystem.ConsoleTest
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var config = ConfigurationProvider.Configuration();
            var databaseSettings = config.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
            optionsBuilder.UseSqlite(databaseSettings.GetConnectionString());

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
