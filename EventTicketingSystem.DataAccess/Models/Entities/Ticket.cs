using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Ticket
    {
        public int Id { get; set; }
        public int EventSeatId { get; set; }
        public int UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public TicketStatus Status { get; set; }
        public TicketLevel TicketLevel { get; set; }
        public decimal Price { get; set; }
    }
}
