﻿using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Seat : IEntity
    {
        public int Id { get; set; }
        public int? VenueId { get; set; }
        public string Name { get; set; }
        public int? SectionId { get; set; }
        public int RowNumber { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Type { get; set; }
        public virtual Section Section { get; set; }
        public virtual Venue Venue { get; set; }
    }
}
