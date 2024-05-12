using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<ICollection<Event>> GetEvents();
    Task<ICollection<Event>> GetEventsByCity(int cityId);
    Task<ICollection<Event>> GetEventsByVenue(int venueId);
}