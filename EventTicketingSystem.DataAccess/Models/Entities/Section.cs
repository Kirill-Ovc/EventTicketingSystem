namespace EventTicketingSystem.DataAccess.Models.Entities
{
    internal class Section
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
    }
}
