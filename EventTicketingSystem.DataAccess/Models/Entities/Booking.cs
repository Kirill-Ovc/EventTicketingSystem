using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Booking
    {
        public int Id { get; set; }
        public int EventSeatId { get; set; }
        public int UserId { get; set; }
        public BookingStatus Status { get; set; }
        public decimal Price { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationTimeStamp { get; set; }
    }
}
