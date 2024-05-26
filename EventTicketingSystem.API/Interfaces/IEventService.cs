using EventTicketingSystem.API.Models;

namespace EventTicketingSystem.API.Interfaces
{
    public interface IEventService
    {
        Task<IList<EventDto>> GetAllEvents();
        Task<IList<EventDto>> GetEventsByCity(int cityId);
        Task<IList<EventSeatDto>> GetEventSeats(int eventId, int sectionId);
    }
}