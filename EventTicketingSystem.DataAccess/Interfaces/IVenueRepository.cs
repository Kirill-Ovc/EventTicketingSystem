using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IVenueRepository : IRepository<Venue>
{
    Task<ICollection<Venue>> GetVenues();
    Task<ICollection<Venue>> GetVenuesByCity(int cityId);
}