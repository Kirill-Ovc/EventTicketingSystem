using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class CityRepositoryTests: IDisposable
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
            Assert.AreEqual(1, city.Id);
            Assert.AreEqual("Moscow", city.Name);
            Assert.AreEqual("Russia", city.Country);
        }

        [Test]
        public async Task CityRepository_GetCities_ReturnsCities()
        {
            var expectedCities = await _dataProvider.GetCities();
            var cities = (await _cityRepository.GetCities()).ToList();

            Assert.IsNotNull(cities);
            Assert.AreEqual(expectedCities.Count, cities.Count);
            for (int i = 0; i < cities.Count; i++)
            {
                Assert.AreEqual(expectedCities[i].Name, cities[i].Name);
                Assert.AreEqual(expectedCities[i].Country, cities[i].Country);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
