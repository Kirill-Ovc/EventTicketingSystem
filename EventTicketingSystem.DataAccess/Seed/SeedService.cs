using System.Text.Json;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Seed
{
    internal class SeedService : ISeedService
    {
        private readonly DatabaseContext _context;
        private readonly ISeatRepository _seatRepository;

        public SeedService(DatabaseContext context,
            ISeatRepository seatRepository)
        {
            _context = context;
            _seatRepository = seatRepository;
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
        }

        public async Task SeedAllData()
        {
            await SeedCities();
            await SeedUsers();
            await CreateVenueWithSeats();
            await CreateEventWithSeats();
        }

        public async Task SeedUsers()
        {
            if (await _context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Seed/Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<User>>(userData, options);

            foreach (var user in users)
            {
                user.Username = user.Username.ToLower();
                user.Password = "Password";
                await _context.Users.AddAsync(user);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SeedCities()
        {
            if (await _context.Cities.AnyAsync()) return;

            var cityData = await File.ReadAllTextAsync("Seed/Data/CitiesSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cities = JsonSerializer.Deserialize<List<City>>(cityData, options);

            await _context.AddRangeAsync(cities);
            await _context.SaveChangesAsync();
        }

        private async Task CreateVenueWithSeats()
        {
            if (await _context.Venues.AnyAsync()) return;

            var venue = new Venue
            {
                Name = "Red square",
                CityId = 1,
                Address = "Red square, Moscow",
                Information = "Red square"
            };
            await _context.AddAsync(venue);

            var sectionA = new Section
            {
                Id = 1,
                VenueId = 1,
                Name = "Section A",
                Capacity = 100
            };
            var sectionB = new Section
            {
                Id = 2,
                VenueId = 1,
                Name = "Section B",
                Capacity = 200
            };
            await _context.AddAsync(sectionA);
            await _context.AddAsync(sectionB);


            // Seats in Venue
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    var seat = new Seat
                    {
                        SectionId = 1,
                        Name = "A" + i + "-" + j,
                        RowNumber = i,
                        VenueId = 1
                    };
                    await _context.AddAsync(seat);
                }
            }

            for (int i = 1; i <= 20; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    var seat = new Seat
                    {
                        SectionId = 2,
                        Name = "B" + i + "-" + j,
                        RowNumber = i,
                        VenueId = 1
                    };
                    await _context.AddAsync(seat);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateEventWithSeats()
        {
            if (await _context.EventInfos.AnyAsync()) return;

            var eventData = new EventInfo()
            {
                Id = 1,
                Name = "Linkin Park Concert",
                Information = "Linkin Park concert on Red square",
                Type = "Concert"
            };
            await _context.AddAsync(eventData);

            var event1 = new Event
            {
                Id = 1,
                VenueId = 1,
                EventInfoId = 1,
                Time = new DateTime(2024, 06, 01, 12, 0, 0),
                Name = "Linkin Park Concert 1",
            };
            var event2 = new Event
            {
                Id = 2,
                VenueId = 1,
                EventInfoId = 1,
                Time = new DateTime(2024, 06, 03, 12, 0, 0),
                Name = "Linkin Park Concert 2",
            };
            await _context.AddAsync(event1);
            await _context.AddAsync(event2);

            await _seatRepository.CreateEventSeats(1, 1);

            await _context.SaveChangesAsync();
        }

    }
}
