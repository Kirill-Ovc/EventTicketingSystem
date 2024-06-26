﻿using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class Venue : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Information { get; set; }
        public string PhotoUrl { get; set; }
        public string SeatMapUrl { get; set; }
        public virtual City City { get; set; }
    }
}
