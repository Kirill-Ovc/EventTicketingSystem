using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

public interface IEventRepository
{
    Task<ICollection<Event>> GetEvents();
    Task<Event> GetEventById(int id);
    Task<ICollection<Event>> GetEventsByCity(int cityId);
    Task<ICollection<Event>> GetEventsByVenue(int venueId);
    Task Add(Event @event);
    void Update(Event @event);
    void Delete(int id);
}