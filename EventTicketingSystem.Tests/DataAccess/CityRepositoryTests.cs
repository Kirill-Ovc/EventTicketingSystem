using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class CityRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private ICityRepository _cityRepository;
        private DatabaseContext _context;

        public CityRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _cityRepository = new CityRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedCities();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task CityRepository_Find_ReturnsCity()
        {
            var city = await _cityRepository.Find(1);

            city.Should().NotBeNull();
            city.Id.Should().Be(1);
            city.Name.Should().Be("Moscow");
            city.Country.Should().Be("Russia");
        }

        [Test]
        public async Task CityRepository_GetCities_ReturnsCities()
        {
            var expectedCities = await _dataProvider.GetCities();
            var cities = (await _cityRepository.GetCities()).ToList();

            cities.Should().NotBeNull();
            cities.Should().HaveCount(expectedCities.Count);
            for (int i = 0; i < cities.Count; i++)
            {
                cities[i].Name.Should().Be(expectedCities[i].Name);
                cities[i].Country.Should().Be(expectedCities[i].Country);
            }
        }

        [Test]
        public async Task CityRepository_Add_AddsCity()
        {
            var city = new City
            {
                Name = "New City",
                Country = "New Country"
            };

            await _cityRepository.Add(city);
            await _cityRepository.SaveChanges();

            var addedCity = await _cityRepository.Find(city.Id);

            addedCity.Should().NotBeNull();
            addedCity.Name.Should().Be(city.Name);
            addedCity.Country.Should().Be(city.Country);
        }

        [Test]
        public async Task CityRepository_Update_UpdatesCity()
        {
            var city = await _cityRepository.Find(1);
            city.Name = "Updated City";
            city.Country = "Updated Country";

            await _cityRepository.Update(city);
            await _cityRepository.SaveChanges();

            var updatedCity = await _cityRepository.Find(city.Id);

            updatedCity.Should().NotBeNull();
            updatedCity.Name.Should().Be(city.Name);
            updatedCity.Country.Should().Be(city.Country);
        }

        [Test]
        public async Task CityRepository_Delete_DeletesCity()
        {
            var existingCity = await _cityRepository.Find(1);

            Assert.IsNotNull(existingCity);

            await _cityRepository.Delete(1);
            await _cityRepository.SaveChanges();

            var deletedCity = await _cityRepository.Find(1);

            deletedCity.Should().BeNull();
        }

        [Test]
        public void CityRepository_Delete_WhenCityDoesNotExist_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _cityRepository.Delete(100));
            Assert.DoesNotThrowAsync(async () => await _cityRepository.SaveChanges());
        }
    
    }
}
