using EventTicketingSystem.DataAccess.Interfaces;
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

        public async Task<City> Find(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

        public async Task Add(City city)
        {
            await _context.AddAsync(city);
        }

        public Task Update(City city)
        {
            _context.Cities.Update(city);
            return Task.CompletedTask;
        }

        public async Task Delete(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
