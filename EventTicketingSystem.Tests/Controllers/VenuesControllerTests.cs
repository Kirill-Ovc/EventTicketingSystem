using AutoFixture;
using EventTicketingSystem.API.Controllers;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace EventTicketingSystem.Tests.Controllers
{
    [TestFixture]
    public class VenuesControllerTests
    {
        private VenuesController _controller;
        private IVenueRepository _venueRepository;
        private ISeatRepository _seatRepository;
        private readonly IFixture _fixture = new Fixture();

        public VenuesControllerTests()
        {
            _fixture.Customize<Section>(s => s
                .Without(b => b.Venue)
                .Without(b => b.Seats));
        }

        [SetUp]
        public void Setup()
        {
            _venueRepository = Substitute.For<IVenueRepository>();
            _seatRepository = Substitute.For<ISeatRepository>();
            _controller = new VenuesController(_venueRepository, _seatRepository);
        }

        [Test]
        public async Task VenuesController_Get_ReturnsVenues()
        {
            // Arrange
            var venues = _fixture.CreateMany<Venue>(10).ToList();
            _venueRepository.GetVenues().Returns(venues);

            // Act
            var response = await _controller.Get();
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(venues);
        }

        [Test]
        public async Task VenuesController_GetByCity_ReturnsVenuesByCity()
        {
            // Arrange
            var city = _fixture.Create<int>();
            var venues = _fixture.CreateMany<Venue>(5).ToList();
            _venueRepository.GetVenuesByCity(city).Returns(venues);

            // Act
            var response = await _controller.GetByCity(city);
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(venues);
        }

        [Test]
        public async Task VenuesController_GetSections_ReturnsSections()
        {
            // Arrange
            var venueId = _fixture.Create<int>();
            var sections = _fixture.CreateMany<Section>(5).ToList();
            _seatRepository.GetSections(venueId).Returns(sections);

            // Act
            var response = await _controller.GetSections(venueId);
            var okResult = response as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(sections);
        }

        [Test]
        public async Task VenuesController_GetSections_WhenVenueIdIsZero_ReturnsBadRequest()
        {
            // Arrange
            var venueId = 0;

            // Act
            var response = await _controller.GetSections(venueId);
            var badRequestResult = response as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().Be("Venue ID is required");
        }

        [Test]
        public async Task VenuesController_GetByCity_WhenCityIdIsZero_ReturnsBadRequest()
        {
            // Arrange
            var cityId = 0;

            // Act
            var response = await _controller.GetByCity(cityId);
            var badRequestResult = response as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().Be("City ID is required");
        }
    }
}
