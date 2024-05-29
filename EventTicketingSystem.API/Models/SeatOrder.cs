namespace EventTicketingSystem.API.Models
{
    public class SeatOrder
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int EventSeatId { get; set; }
        public int OfferId { get; set;}
    }
}
