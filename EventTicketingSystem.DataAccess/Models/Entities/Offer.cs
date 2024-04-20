using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Offer
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public TicketLevel TicketLevel { get; set; }
        public decimal Price { get; set; }
    }
}
