using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class VenueRepository : BaseRepository<Venue>, IVenueRepository
    {
        private readonly DatabaseContext _context;

        public VenueRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Venue>> GetVenues()
        {
            return await _context.Venues.ToListAsync();
        }

        public async Task<ICollection<Venue>> GetVenuesByCity(int cityId)
        {
            return await _context.Venues.Where(v => v.CityId == cityId).ToListAsync();
        }
    }
}
