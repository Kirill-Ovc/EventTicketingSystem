namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public int VenueId { get; set; }
        public int EventInfoId { get; set; }
        public virtual Venue Venue { get; set; }
        public virtual EventInfo EventInfo { get; set; }
    }
}
