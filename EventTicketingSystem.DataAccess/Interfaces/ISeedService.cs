namespace EventTicketingSystem.DataAccess.Interfaces;

public interface ISeedService
{
    Task SeedAllData();
    Task SeedUsers();
    Task SeedCities();
}