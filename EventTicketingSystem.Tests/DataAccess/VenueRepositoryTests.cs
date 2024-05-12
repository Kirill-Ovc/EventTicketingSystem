using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;
using System;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class VenueRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IVenueRepository _venueRepository;
        private DatabaseContext _context;

        public VenueRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _venueRepository = new VenueRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedCities();
            await seeder.SeedVenues();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task VenueRepository_Find_ReturnsVenue()
        {
            var venues = _dataProvider.GetVenues();
            var expectedVenue = venues[0];
            var venue = await _venueRepository.Find(expectedVenue.Id);

            venue.Should().BeEquivalentTo(expectedVenue, options => options.Excluding(x => x.City));
        }

        [Test]
        public async Task VenueRepository_Add_AddsVenue()
        {
            var venue = new Venue()
            {
                Name = "New Venue",
                Address = "Address",
                Information = "Information",
                CityId = 1
            };

            await _venueRepository.Add(venue);
            await _venueRepository.SaveChanges();

            var addedVenue = await _venueRepository.Find(venue.Id);

            addedVenue.Should().NotBeNull();
            addedVenue.Should().BeEquivalentTo(venue, options => options.Excluding(x => x.City));
        }

        [Test]
        public async Task VenueRepository_Update_UpdatesVenue()
        {
            var venue = await _venueRepository.Find(1);
            venue.Name = "Updated Name";
            venue.Information = "New Information";

            await _venueRepository.Update(venue);
            await _venueRepository.SaveChanges();

            var updatedVenue = await _venueRepository.Find(venue.Id);

            updatedVenue.Should().NotBeNull();
            updatedVenue.Should().BeEquivalentTo(venue, options => options.Excluding(x => x.City));
        }

        [Test]
        public async Task VenueRepository_Delete_DeletesVenue()
        {
            var existingVenue = await _venueRepository.Find(1);

            Assert.IsNotNull(existingVenue);

            await _venueRepository.Delete(1);
            await _venueRepository.SaveChanges();

            var deletedVenue = await _venueRepository.Find(1);

            deletedVenue.Should().BeNull();
        }

        [Test]
        public async Task VenueRepository_GetVenues_ReturnsAllVenues()
        {
            var expectedVenues = _dataProvider.GetVenues();
            var venues = (await _venueRepository.GetVenues()).OrderBy(x => x.Id).ToList();

            venues.Should().BeEquivalentTo(expectedVenues, options => options.Excluding(x => x.City));
        }

        [Test]
        public async Task VenueRepository_GetVenuesByCity_ReturnsVenues()
        {
            var expectedVenues = _dataProvider.GetVenues().Where(v => v.CityId == 1).ToList();
            var venues = (await _venueRepository.GetVenuesByCity(1)).OrderBy(x => x.Id).ToList();

            venues.Should().BeEquivalentTo(expectedVenues, options => options.Excluding(x => x.City));
        }

        [Test]
        public async Task VenueRepository_Find_WhenNotExist_ReturnsNull()
        {
            var venue = await _venueRepository.Find(100);
            venue.Should().BeNull();
        }

        [Test]
        public async Task VenueRepository_Update_WhenNotExist_ThrowsException()
        {
            var venue = await _venueRepository.Find(1);
            var newVenue = new Venue
            {
                Id = 100,
                Name = "Updated Name",
                Information = "New Information",
                CityId = venue.CityId,
                Address = venue.Address,
            };

            Assert.That(async () =>
            {
                await _venueRepository.Update(newVenue);
                await _venueRepository.SaveChanges();
            }, Throws.Exception.TypeOf<InvalidOperationException>());

            var updatedVenue = await _venueRepository.Find(newVenue.Id);

            updatedVenue.Should().BeNull();
        }

        [Test]
        public void VenueRepository_Delete_WhenVenueDoesNotExist_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _venueRepository.Delete(100));
            Assert.DoesNotThrowAsync(async () => await _venueRepository.SaveChanges());
        }
    }
}
