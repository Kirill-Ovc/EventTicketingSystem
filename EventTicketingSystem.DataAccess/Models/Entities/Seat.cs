namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Seat
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Type { get; set; }
    }
}
