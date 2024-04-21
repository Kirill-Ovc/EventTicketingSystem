﻿using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{

    internal class CityRepository : ICityRepository
    {
        private readonly DatabaseContext _context;

        public CityRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ICollection<City>> GetCities()
        {
            return await _context.Cities.ToListAsync();
        }
    }
}
