using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface IOfferRepository : IRepository<Offer>
    {
        Task<ICollection<Offer>> GetOffersByEvent(int eventId);
    }
}
