using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface IEventInfoRepository: IRepository<EventInfo>
{
    Task<ICollection<EventInfo>> GetEvents();
    Task<ICollection<EventInfo>> GetEventsByCity(int cityId);
    Task<ICollection<EventInfo>> GetEventsByVenue(int venueId);
}