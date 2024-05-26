namespace EventTicketingSystem.API.Models
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DataAndTime { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public int EventInfoId { get; set; }
        public string Type { get; set; }
        public string PosterUrl { get; set; }
    }
}
