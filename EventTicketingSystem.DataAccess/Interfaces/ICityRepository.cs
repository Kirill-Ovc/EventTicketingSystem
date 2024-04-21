using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface ICityRepository
    {
        Task<ICollection<City>> GetCities();
    }
}
