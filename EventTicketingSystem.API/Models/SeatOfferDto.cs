using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Models
{
    public class SeatOfferDto
    {
        public int OfferId { get; set; }

        public TicketLevel TicketLevel { get; set; }

        public decimal Price { get; set; }
    }
}
