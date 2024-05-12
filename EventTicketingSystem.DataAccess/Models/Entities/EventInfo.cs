using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class EventInfo : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Information { get; set; }
        public string Type { get; set; }
        public string PosterUrl { get; set; }

        public ICollection<Event> EventOccurrences { get; set; }
    }
}
