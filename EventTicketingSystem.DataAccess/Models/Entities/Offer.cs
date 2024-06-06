using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Offer : IEntity
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public TicketLevel TicketLevel { get; set; }
        public decimal Price { get; set; }
        public virtual Event Event { get; set; }
        public virtual Section Section { get; set; }
    }
}
