using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using Microsoft.EntityFrameworkCore;
using EventInfo = EventTicketingSystem.DataAccess.Models.Entities.EventInfo;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class EventInfoRepository : IEventInfoRepository
    {
        private readonly DatabaseContext _context;

        public EventInfoRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ICollection<EventInfo>> GetEvents()
        {
            return await _context.EventInfos
                .Include(e => e.EventOccurrences)
                .ToListAsync();
        }

        public async Task<EventInfo> GetEventById(int id)
        {
            return await _context.EventInfos
                .Include(e => e.EventOccurrences)
                .SingleOrDefaultAsync(e => e.Id == id);

        }

        public async Task<ICollection<EventInfo>> GetEventsByCity(int cityId)
        {
            var eventIds = await _context.Events.Where(e => e.Venue.CityId == cityId)
                .Select(e => e.EventInfoId)
                .Distinct()
            .ToListAsync();

            return await _context.EventInfos.Where(e => eventIds.Contains(e.Id)).ToListAsync();
        }

        public async Task<ICollection<EventInfo>> GetEventsByVenue(int venueId)
        {
            var eventIds = await _context.Events.Where(e => e.VenueId == venueId)
                .Select(e => e.EventInfoId)
                .Distinct()
                .ToListAsync();

            return await _context.EventInfos.Where(e => eventIds.Contains(e.Id)).ToListAsync();
        }

        public async Task AddEventInfo(EventInfo eventInfo)
        {
            await _context.AddAsync(eventInfo);
        }

        public void UpdateEventInfo(EventInfo eventInfo)
        {
            _context.EventInfos.Update(eventInfo);
        }

        public void DeleteEventInfo(int id)
        {
            var eventInfo = _context.EventInfos.Find(id);
            if (eventInfo != null)
            {
                _context.RemoveRange(eventInfo.EventOccurrences);
                _context.EventInfos.Remove(eventInfo);
            }
        }

    }
}
