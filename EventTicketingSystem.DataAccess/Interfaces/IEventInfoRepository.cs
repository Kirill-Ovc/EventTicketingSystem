using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface IEventInfoRepository
{
    Task<ICollection<EventInfo>> GetEvents();
    Task<EventInfo> GetEventById(int id);
    Task<ICollection<EventInfo>> GetEventsByCity(int cityId);
    Task<ICollection<EventInfo>> GetEventsByVenue(int venueId);
    Task AddEventInfo(EventInfo eventInfo);
    void UpdateEventInfo(EventInfo eventInfo);
    void DeleteEventInfo(int id);
}