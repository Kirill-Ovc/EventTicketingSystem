using AutoFixture;
using EventTicketingSystem.API.Controllers;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace EventTicketingSystem.Tests.Controllers
{
    [TestFixture]
    public class EventsControllerTests
    {
        private EventsController _controller;
        private IEventService _eventService;
        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _eventService = Substitute.For<IEventService>();
            _controller = new EventsController(_eventService);
        }

        [Test]
        public async Task EventsController_Get_ReturnsEvents()
        {
            // Arrange
            var events = _fixture.CreateMany<EventDto>(10).ToList();
            _eventService.GetAllEvents().Returns(events);

            // Act
            var response = await _controller.Get();
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(events);
            await _eventService.Received().GetAllEvents();
        }

        [Test]
        public async Task EventsController_GetByCity_ReturnsEventsByCity()
        {
            // Arrange
            var city = _fixture.Create<int>();
            var events = _fixture.CreateMany<EventDto>(5).ToList();
            _eventService.GetEventsByCity(city).Returns(events);

            // Act
            var response = await _controller.GetByCity(city);
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(events);
            await _eventService.Received().GetEventsByCity(city);
        }

        [Test]
        public async Task EventsController_GetSeats_ReturnsSeats()
        {
            // Arrange
            var eventId = 1;
            var sectionId = 1;
            var seats = _fixture.CreateMany<EventSeatDto>(100).ToList();
            _eventService.GetEventSeats(eventId, sectionId).Returns(seats);

            // Act
            var response = await _controller.GetSeats(eventId, sectionId);
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(seats);
            await _eventService.Received().GetEventSeats(eventId, sectionId);
        }

        [Test]
        public async Task EventsController_GetSeats_NullParams_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetSeats(null, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Event ID and Section ID are required");
            await _eventService.DidNotReceive().GetEventSeats(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public async Task EventsController_GetByCity_CityIdZero_ReturnsEvents()
        {
            // Arrange
            var cityId = 0;
            var events = _fixture.CreateMany<EventDto>(5).ToList();
            _eventService.GetEventsByCity(cityId).Returns(events);

            // Act
            var result = await _controller.GetByCity(cityId);
            
            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(events);
        }

        [Test]
        public async Task EventsController_GetByCity_WhenCityIdIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetByCity(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("City ID is required");
        }
    }
}
