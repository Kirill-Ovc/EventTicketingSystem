using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private readonly DatabaseContext _context;

        public EventRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Event>> GetEvents()
        {
            return await _context.Events
                .Include(e => e.EventInfo)
                .Include(e => e.Venue)
                .ToListAsync();
        }

        public async Task<ICollection<Event>> GetEventsByCity(int cityId)
        {
            return await _context.Events
                .Include(e => e.EventInfo)
                .Include(e => e.Venue)
                .Where(e => e.Venue.CityId == cityId).ToListAsync();
        }

        public async Task<ICollection<Event>> GetEventsByVenue(int venueId)
        {
            return await _context.Events
                .Include(e => e.EventInfo)
                .Include(e => e.Venue)
                .Where(e => e.VenueId == venueId).ToListAsync();
        }
    }
}
