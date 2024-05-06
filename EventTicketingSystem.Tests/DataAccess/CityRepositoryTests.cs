using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;

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

            Assert.IsNotNull(city);
            Assert.That(city.Id, Is.EqualTo(1));
            Assert.That(city.Name, Is.EqualTo("Moscow"));
            Assert.That(city.Country, Is.EqualTo("Russia"));
        }

        [Test]
        public async Task CityRepository_GetCities_ReturnsCities()
        {
            var expectedCities = await _dataProvider.GetCities();
            var cities = (await _cityRepository.GetCities()).ToList();

            Assert.IsNotNull(cities);
            Assert.That(cities.Count, Is.EqualTo(expectedCities.Count));
            for (int i = 0; i < cities.Count; i++)
            {
                Assert.That(cities[i].Name, Is.EqualTo(expectedCities[i].Name));
                Assert.That(cities[i].Country, Is.EqualTo(expectedCities[i].Country));
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

            Assert.IsNotNull(addedCity);
            Assert.That(addedCity.Name, Is.EqualTo(city.Name));
            Assert.That(addedCity.Country, Is.EqualTo(city.Country));
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

            Assert.IsNotNull(updatedCity);
            Assert.That(updatedCity.Name, Is.EqualTo(city.Name));
            Assert.That(updatedCity.Country, Is.EqualTo(city.Country));
        }

        [Test]
        public async Task CityRepository_Delete_DeletesCity()
        {
            var existingCity = await _cityRepository.Find(1);

            Assert.IsNotNull(existingCity);

            await _cityRepository.Delete(1);
            await _cityRepository.SaveChanges();

            var deletedCity = await _cityRepository.Find(1);

            Assert.IsNull(deletedCity);
        }
    }
}
