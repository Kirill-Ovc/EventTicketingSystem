using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class SeatRepository : ISeatRepository
    {
        private readonly DatabaseContext _context;

        public SeatRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Section>> GetSections(int eventId)
        {
            var sectionIds = await _context.EventSeats
                .Where(s => s.EventId == eventId)
                .Select(s => s.Seat.SectionId)
                .Distinct()
                .ToListAsync();

            return await _context.Sections.Where(s => sectionIds.Contains(s.Id)).ToListAsync();
        }

        public async Task<ICollection<EventSeat>> GetEventSeats(int eventId, int sectionId)
        {
            return await _context.EventSeats
                .Include(s => s.Seat)
                .Where(s => s.EventId == eventId && s.Seat.SectionId == sectionId)
                .ToListAsync();
        }

        public async Task<ICollection<Seat>> GetVenueSeats(int venueId)
        {
            return await _context.Seats.Where(s => s.VenueId == venueId).ToListAsync();
        }

        public async Task CreateEventSeats(int eventId, int venueId)
        {
            var venueSeats = await GetVenueSeats(venueId);
            foreach (var seat in venueSeats)
            {
                var eventSeat = new EventSeat
                {
                    EventId = eventId,
                    SeatId = seat.Id,
                    Name = seat.Name,
                    Status = EventSeatStatus.Available
                };
                await _context.EventSeats.AddAsync(eventSeat);
            }
        }
    }
}
