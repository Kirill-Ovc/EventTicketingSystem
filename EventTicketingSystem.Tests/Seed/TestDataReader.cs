using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventTicketingSystem.Tests.Seed
{
    internal class TestDataReader
    {
        public async Task<List<User>> GetUsers()
        {
            var userData = await File.ReadAllTextAsync("Seed/TestData/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<User>>(userData, options);
            return users;
        }

        public async Task<List<City>> GetCities()
        {
            var cityData = await File.ReadAllTextAsync("Seed/TestData/CitySeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var cities = JsonSerializer.Deserialize<List<City>>(cityData, options);
            return cities;
        }

        public List<Venue> GetVenues()
        {
            var venues = new List<Venue>()
            {
                new Venue
                {
                    Name = "Tinkoff Arena",
                    CityId = 2,
                    Address = "Primorsky prospect, St. Petersburg, Russia",
                    Information = "Concert hall"
                },
                new Venue
                {
                    Name = "Red square",
                    CityId = 1,
                    Address = "Red square, Moscow, Russia",
                    Information = "Red square"
                },
                new Venue
                {
                    Name = "Antalya Expo Kongre Merkezi",
                    CityId = 3,
                    Address = "Solak, 07112 Aksu/Antalya, Türkiye",
                    Information = "Conference center"
                },
                new Venue
                {
                    Name = "Moscone Center",
                    CityId = 4,
                    Address = "San Francisco, California, United States",
                    Information = "The George R. Moscone Convention Center"
                }
            };
            return venues;
        }

        public List<Section> GetSectionsWithSeats(int venueId)
        {
            var sectionA = new Section
            {
                Id = 1,
                VenueId = venueId,
                Name = "Section A",
                Capacity = 200,
                Seats = GetSeatsForSection(venueId, 1, 20, "A").ToList()
            };
            var sectionB = new Section
            {
                Id = 2,
                VenueId = venueId,
                Name = "Section B",
                Capacity = 100,
                Seats = GetSeatsForSection(venueId, 2, 10, "B").ToList()
            };
            var sectionC = new Section
            {
                Id = 3,
                VenueId = venueId,
                Name = "Section C",
                Capacity = 100,
                Seats = GetSeatsForSection(venueId, 3,10, "C").ToList()
            };
            return new List<Section> { sectionA, sectionB, sectionC };
        }

        private IEnumerable<Seat> GetSeatsForSection(int venueId, int sectionId, int sectionSize, string sectionLetter)
        {
            for (int i = 1; i <= sectionSize; i++)
            {
                for (int j = 1; j <= sectionSize; j++)
                {
                    yield return new Seat
                    {
                        SectionId = sectionId,
                        Name = sectionLetter + i + "-" + j,
                        RowNumber = i,
                        VenueId = venueId
                    };
                }
            }
        }

        public List<EventInfo> GetEvents()
        {
            var event1 = new EventInfo()
            {
                Id = 1,
                Name = "Famous Concert 1",
                Information = "Concert of famous artist",
                Type = "Concert",
                EventOccurrences = new List<Event>()
                {
                    new Event
                    {
                        VenueId = 1,
                        EventInfoId = 1,
                        DataAndTime = new DateTime(2024, 05, 01, 16, 0, 0, DateTimeKind.Utc),
                        Name = "Concert of famous artist",
                    },
                    new Event
                    {
                        VenueId = 3,
                        EventInfoId = 1,
                        DataAndTime = new DateTime(2024, 05, 09, 16, 0, 0, DateTimeKind.Utc),
                        Name = "Concert of famous artist",
                    }
                }
            };

            var event2 = new EventInfo()
            {
                Id = 2,
                Name = "Linkin Park Concert",
                Information = "Linkin Park concert",
                Type = "Concert",
                EventOccurrences = new List<Event>()
                {
                    new Event
                    {
                        VenueId = 2,
                        EventInfoId = 2,
                        DataAndTime = new DateTime(2024, 06, 01, 14, 0, 0, DateTimeKind.Utc),
                        Name = "Linkin Park Concert 1 on Red square",
                    },
                    new Event
                    {
                        VenueId = 2,
                        EventInfoId = 2,
                        DataAndTime = new DateTime(2024, 06, 03, 14, 0, 0, DateTimeKind.Utc),
                        Name = "Linkin Park Concert 2 on Red square",
                    },
                    new Event
                    {
                        VenueId = 1,
                        EventInfoId = 2,
                        DataAndTime = new DateTime(2024, 06, 10, 14, 0, 0, DateTimeKind.Utc),
                        Name = "Linkin Park Concert 3 in Tinkoff Arena",
                    },
                }
            };

            var event3 = new EventInfo()
            {
                Id = 3,
                Name = "Macworld Conference",
                Information = "Apple iPhone Conference",
                Type = "Conference",
                EventOccurrences = new List<Event>()
                {
                    new Event
                    {
                        VenueId = 4,
                        EventInfoId = 3,
                        DataAndTime = new DateTime(2024, 09, 05, 12, 0, 0, DateTimeKind.Utc),
                        Name = "Macworld Conference",
                    },
                }
            };
            return new List<EventInfo>() { event1, event2, event3 };
        }

        public List<EventSeat> GetEventSeats(List<Seat> venueSeats, int eventId)
        {
            var eventSeats = new List<EventSeat>();
            foreach (var seat in venueSeats)
            {
                var eventSeat = new EventSeat
                {
                    EventId = eventId,
                    SeatId = seat.Id,
                    Name = seat.Name,
                    Status = EventSeatStatus.Available
                };
                eventSeats.Add(eventSeat);
            }
            return eventSeats;
        }
    }
}
