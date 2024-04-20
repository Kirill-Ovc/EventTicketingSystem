namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Information { get; set; }
        public string PhotoUrl { get; set; }
        public string SeatMapUrl { get; set; }
    }
}
