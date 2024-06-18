using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;

namespace EventTicketingSystem.Tests.Integration.Seed
{
    internal class TestDataProvider
    {
        public List<Venue> GetVenues()
        {
            var venues = new List<Venue>()
            {
                new Venue
                {
                    Id = 1,
                    Name = "Tinkoff Arena",
                    CityId = 1,
                    Address = "Primorsky prospect, St. Petersburg, Russia",
                    Information = "Concert hall"
                },
                new Venue
                {
                    Id = 2,
                    Name = "Red square",
                    CityId = 2,
                    Address = "Red square, Moscow, Russia",
                    Information = "Red square"
                },
                new Venue
                {
                    Id = 3,
                    Name = "Antalya Expo Kongre Merkezi",
                    CityId = 3,
                    Address = "Solak, 07112 Aksu/Antalya, Türkiye",
                    Information = "Conference center"
                },
                new Venue
                {
                    Id = 4,
                    Name = "Moscone Center",
                    CityId = 10,
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
                Capacity = 400,
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
                Seats = GetSeatsForSection(venueId, 3, 10, "C").ToList()
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
                        Id = 11,
                        VenueId = 1,
                        EventInfoId = 1,
                        DataAndTime = new DateTime(2024, 05, 01, 16, 0, 0, DateTimeKind.Utc),
                        Name = "Concert of famous artist",
                    },
                    new Event
                    {
                        Id = 12,
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
                        Id = 21,
                        VenueId = 2,
                        EventInfoId = 2,
                        DataAndTime = new DateTime(2024, 06, 01, 14, 0, 0, DateTimeKind.Utc),
                        Name = "Linkin Park Concert 1 on Red square",
                    },
                    new Event
                    {
                        Id = 22,
                        VenueId = 2,
                        EventInfoId = 2,
                        DataAndTime = new DateTime(2024, 06, 03, 14, 0, 0, DateTimeKind.Utc),
                        Name = "Linkin Park Concert 2 on Red square",
                    },
                    new Event
                    {
                        Id = 23,
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
                        Id = 31,
                        VenueId = 4,
                        EventInfoId = 3,
                        DataAndTime = new DateTime(2024, 09, 05, 12, 0, 0, DateTimeKind.Utc),
                        Name = "Macworld Conference",
                    },
                }
            };
            return new List<EventInfo>() { event1, event2, event3 };
        }

        public List<Offer> GetOffers(int eventId, List<Section> sections)
        {
            var offers = sections.Select(s =>
                new Offer()
                {
                    EventId = eventId,
                    SectionId = s.Id,
                    RowNumber = 1,
                    TicketLevel = TicketLevel.Other,
                    Price = 100
                })
                .ToList();
            return offers;
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

        //sea

        public List<Booking> GetBookings()
        {
            var bookings = new List<Booking>();
            for (int i = 1; i <= 5; i++)
            {
                var booking = new Booking
                {
                    UserId = i,
                    Status = BookingStatus.Active,
                    ExpirationTimeStamp = DateTime.Today.AddDays(1),
                    Price = 1000
                };
                bookings.Add(booking);
            }
            for (int i = 1; i <= 5; i++)
            {
                var booking = new Booking
                {
                    UserId = i,
                    Status = BookingStatus.Expired,
                    ExpirationTimeStamp = DateTime.Today.AddDays(-1),
                    Price = 900
                };
                bookings.Add(booking);
            }

            return bookings;
        }

        public List<Payment> GetPayments()
        {
            var payments = new List<Payment>();
            for (int i = 1; i <= 5; i++)
            {
                var payment = new Payment
                {
                    Id = i,
                    BookingId = i,
                    Amount = 1000,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentDate = DateTime.Today,
                    PaymentMethod = PaymentMethod.CreditCard
                    
                };
                payments.Add(payment);
            }
            for (int i = 1; i <= 5; i++)
            {
                var payment = new Payment
                {
                    Id = 5 + i,
                    BookingId = 5 + i,
                    Amount = 2000,
                    PaymentStatus = PaymentStatus.Pending
                };
                payments.Add(payment);
            }

            return payments;
        }

        public List<Ticket> GetTickets()
        {
            var tickets = new List<Ticket>();
            for (int i = 1; i <= 5; i++)
            {
                var ticket = new Ticket
                {
                    Id = i,
                    EventSeatId = 10 + i,
                    UserId = i,
                    Price = 1000,
                    Status = TicketStatus.Active,
                    TicketLevel = TicketLevel.Adult
                };
                tickets.Add(ticket);
            }

            return tickets;
        }
    }
}
