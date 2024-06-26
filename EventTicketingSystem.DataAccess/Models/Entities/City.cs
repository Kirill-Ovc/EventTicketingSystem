﻿using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
    }
}
