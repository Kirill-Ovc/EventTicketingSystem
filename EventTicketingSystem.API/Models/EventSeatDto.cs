using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.API.Models
{
    public class EventSeatDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public EventSeatStatus Status { get; set; }
        public List<SeatOfferDto> Prices { get; set; }
    }
}
