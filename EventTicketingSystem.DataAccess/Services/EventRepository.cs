using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class EventRepository : IEventRepository
    {
        private readonly DatabaseContext _context;

        public EventRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Event>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventById(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<ICollection<Event>> GetEventsByCity(int cityId)
        {
            return await _context.Events.Where(e => e.Venue.CityId == cityId).ToListAsync();
        }

        public async Task<ICollection<Event>> GetEventsByVenue(int venueId)
        {
            return await _context.Events.Where(e => e.VenueId == venueId).ToListAsync();
        }

        public async Task Add(Event @event)
        {
            await _context.AddAsync(@event);
        }

        public void Update(Event @event)
        {
            _context.Events.Update(@event);
        }

        public void Delete(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }
        }
    }
}
