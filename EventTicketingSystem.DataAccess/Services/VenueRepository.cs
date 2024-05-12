﻿using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class VenueRepository : IVenueRepository
    {
        private readonly DatabaseContext _context;

        public VenueRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Venue> Find(int id)
        {
            return await _context.Venues.FindAsync(id);
        }

        public async Task Add(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
        }

        public async Task Update(Venue venue)
        {
            var entity = await _context.Venues.FindAsync(venue.Id);
            if (entity == null)
            {
                throw new InvalidOperationException("Update failed. Can't find entity by Id");
            }
            _context.Venues.Update(venue);
        }

        public async Task Delete(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Venue>> GetVenues()
        {
            return await _context.Venues.ToListAsync();
        }

        public async Task<ICollection<Venue>> GetVenuesByCity(int cityId)
        {
            return await _context.Venues.Where(v => v.CityId == cityId).ToListAsync();
        }
    }
}