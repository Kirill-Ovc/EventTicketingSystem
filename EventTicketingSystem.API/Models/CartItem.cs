namespace EventTicketingSystem.API.Models
{
    public class CartItem
    {
        public int EventId { get; set; }

        public int EventSeatId { get; set; }

        public string EventName { get; set; }

        public decimal Price { get; set; }
    }
}
