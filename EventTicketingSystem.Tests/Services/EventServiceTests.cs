using AutoFixture;
using AutoMapper;
using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Helpers;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.DataAccess.Services;
using FluentAssertions;
using NSubstitute;

namespace EventTicketingSystem.Tests.Services
{
    [TestFixture]
    public class EventsServiceTests
    {
        private EventService _service;
        private IEventRepository _eventRepository;
        private ISeatRepository _seatRepository;
        private IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        private readonly IFixture _fixture = new Fixture();

        public EventsServiceTests()
        {
            var mapperProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            _mapper = new Mapper(configuration);

            _fixture.Customize<Seat>(e => e
                .Without(f => f.Venue)
                .Without(f => f.Section));
            _fixture.Customize<Event>(e => e
                .Without(f => f.EventInfo));
            _fixture.Customize<EventSeat>(e => e
                .Without(f => f.Event));
        }

        [SetUp]
        public void Setup()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _seatRepository = Substitute.For<ISeatRepository>();
            _offerRepository = Substitute.For<IOfferRepository>();
            _service = new EventService(_eventRepository, _seatRepository, _offerRepository, _mapper);
        }

        [Test]
        public async Task EventService_GetAllEvents_ReturnsEvents()
        {
            // Arrange
            var events = _fixture.CreateMany<Event>(10).ToList();
            _eventRepository.GetEvents().Returns(events);
            events[0] = _testEvent;
            var expectedEvent = _expectedEvent;

            // Act
            var result = await _service.GetAllEvents();

            // Assertx
            result.Should().HaveSameCount(events);
            result[0].Should().BeEquivalentTo(expectedEvent);
            await _eventRepository.Received().GetEvents();
        }

        [Test]
        public async Task EventService_GetByCity_ReturnsEventsByCity()
        {
            // Arrange
            var city = _fixture.Create<int>();
            var events = _fixture.CreateMany<Event>(5).ToList();
            _eventRepository.GetEventsByCity(city).Returns(events);
            events[0] = _testEvent;
            var expectedEvent = _expectedEvent;

            // Act
            var result = await _service.GetEventsByCity(city);

            // Assert
            result.Should().HaveSameCount(events);
            result[0].Should().BeEquivalentTo(expectedEvent);
            await _eventRepository.Received().GetEventsByCity(city);
        }

        [Test]
        public async Task EventService_GetSeats_ReturnsSeats()
        {
            // Arrange
            var eventId = 11;
            var sectionId = 1;
            var seats = _fixture.CreateMany<EventSeat>(100).ToList();

            _eventRepository.Find(eventId).Returns(_testEvent);
            _seatRepository.GetEventSeats(eventId, sectionId).Returns(seats);
            _offerRepository.GetOffersByEvent(eventId).Returns(_testOffers);
            seats[0] = new EventSeat
            {
                Id = 1,
                EventId = eventId,
                SeatId = 123,
                Name = "A1",
                Status = EventSeatStatus.Available,
                Event = _testEvent,
                Seat = new Seat
                {
                    Name = "A1",
                    SectionId = 1,
                    RowNumber = 1,
                    VenueId = 3,
                    PositionX = 32,
                    Section = new Section
                    {
                        Name = "Section 1",
                        VenueId = 1,
                        Venue = new Venue()
                    }
                }
            };
            var expectedSeat = new EventSeatDto
            {
                Id = 1,
                EventId = eventId,
                SeatId = 123,
                Name = "A1",
                Status = EventSeatStatus.Available,
                SectionId = 1,
                RowNumber = 1,
                Prices = _expectedOffers
            };

            // Act
            var result = await _service.GetEventSeats(eventId, sectionId);

            // Assert
            result.Should().HaveSameCount(seats);
            result[0].Should().BeEquivalentTo(expectedSeat);
            await _seatRepository.Received().GetEventSeats(eventId, sectionId);
            await _offerRepository.Received().GetOffersByEvent(eventId);
        }

        [Test]
        public async Task EventService_GetSeats_ThrowsEntityNotFoundException()
        {
            // Arrange
            var eventId = 11;
            var sectionId = 1;
            _eventRepository.Find(eventId).Returns((Event)null);

            // Act
            Func<Task> act = async () => await _service.GetEventSeats(eventId, sectionId);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
            await _seatRepository.DidNotReceive().GetEventSeats(Arg.Any<int>(), Arg.Any<int>());
            await _offerRepository.DidNotReceive().GetOffersByEvent(Arg.Any<int>());
        }

        #region TestData
        private static readonly DateTime _testEventDate = new DateTime(2024, 05, 20, 10, 0, 0, DateTimeKind.Utc);
        private static readonly Event _testEvent = new Event
        {
            Id = 1,
            Name = "Event 1",
            DataAndTime = _testEventDate,
            VenueId = 2,
            EventInfoId = 1,
            Venue = new Venue()
            {
                Id = 2,
                Name = "Venue 1"
            },
            EventInfo = new EventInfo()
            {
                Id = 1,
                Type = "EventType",
                PosterUrl = "PosterUrl"
            }
        };
        private static readonly EventDto _expectedEvent = new EventDto
        {
            Id = 1,
            Name = "Event 1",
            DataAndTime = _testEventDate,
            VenueId = 2,
            VenueName = "Venue 1",
            EventInfoId = 1,
            Type = "EventType",
            PosterUrl = "PosterUrl"
        };
        private static readonly List<Offer> _testOffers = new List<Offer>
        {
            new Offer
            {
                Id = 101,
                EventId = 11,
                SectionId = 1,
                RowNumber = 1,
                TicketLevel = TicketLevel.Adult,
                Price = 100
            },
            new Offer
            {
                Id = 102,
                EventId = 11,
                SectionId = 1,
                RowNumber = 2,
                TicketLevel = TicketLevel.Adult,
                Price = 90
            },
            new Offer
            {
                Id = 103,
                EventId = 11,
                SectionId = 2,
                RowNumber = 1,
                TicketLevel = TicketLevel.Adult,
                Price = 120
            }
        };
        private static readonly List<SeatOfferDto> _expectedOffers = new List<SeatOfferDto>
        {
            new SeatOfferDto
            {
                OfferId = 101,
                TicketLevel = TicketLevel.Adult,
                Price = 100
            }
        };
        #endregion
    }
}


