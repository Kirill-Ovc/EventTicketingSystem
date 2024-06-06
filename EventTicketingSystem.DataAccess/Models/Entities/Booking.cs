using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Booking : IEntity
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public int UserId { get; set; }
        public BookingStatus Status { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpirationTimeStamp { get; set; }
        public virtual User User { get; set; }
    }
}
