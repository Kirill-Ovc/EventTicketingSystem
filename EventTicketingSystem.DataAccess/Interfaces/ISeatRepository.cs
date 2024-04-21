﻿using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface ISeatRepository
{
    Task<ICollection<Section>> GetSections(int eventId);
    Task<ICollection<EventSeat>> GetEventSeats(int eventId, int sectionId);
    Task<ICollection<Seat>> GetVenueSeats(int venueId);
    Task CreateEventSeats(int eventId, int venueId);
}