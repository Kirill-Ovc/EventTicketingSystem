using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class BookingSeat : IEntity
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int EventSeatId { get; set; }
        public decimal Price { get; set; }
        public TicketLevel TicketLevel { get; set; }
        public virtual EventSeat EventSeat { get; set; }
    }
}
