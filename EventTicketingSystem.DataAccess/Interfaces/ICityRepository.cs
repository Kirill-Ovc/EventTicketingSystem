using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface ICityRepository: IRepository<City>
    {
        Task<ICollection<City>> GetCities();
    }
}
