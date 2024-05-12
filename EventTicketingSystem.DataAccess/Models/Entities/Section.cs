using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Section : IEntity
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public int Capacity { get; set; }
        public virtual Venue Venue { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
