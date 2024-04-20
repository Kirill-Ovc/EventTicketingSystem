using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class EventSeat
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public string Name { get; set; }
        public EventSeatStatus Status { get; set; }
    }
}
