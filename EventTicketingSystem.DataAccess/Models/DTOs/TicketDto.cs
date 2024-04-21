using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public TicketStatus Status { get; set; }
        public TicketLevel TicketLevel { get; set; }
        public decimal Price { get; set; }

        public string EventName { get; set; }
        public string EventDateTime { get; set; }
        public string EventVenue { get; set; }
        public string SeatNumber { get; set; }
        public string RowNumber { get; set; }
        public string SectionName { get; set; }
    }
}
