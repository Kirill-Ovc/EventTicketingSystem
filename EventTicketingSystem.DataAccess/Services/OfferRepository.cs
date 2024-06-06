using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class OfferRepository : BaseRepository<Offer>, IOfferRepository
    {
        private readonly DatabaseContext _context;

        public OfferRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Offer>> GetOffersByEvent(int eventId)
        {
            return await _context.Offers.Where(o => o.EventId == eventId).ToListAsync();
        }
    }
}
