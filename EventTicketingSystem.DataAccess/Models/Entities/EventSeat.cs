using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class EventSeat : IEntity
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public string Name { get; set; }
        public EventSeatStatus Status { get; set; }

        public virtual Event Event { get; set; }
        public virtual Seat Seat { get; set; }
    }
}
