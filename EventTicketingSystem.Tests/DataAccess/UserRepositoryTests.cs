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
    public class UserRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IUserRepository _userRepository;
        private DatabaseContext _context;

        public UserRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _userRepository = new UserRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedUsers();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task UserRepository_Find_ReturnsUser()
        {
            var users = await _dataProvider.GetUsers();
            var expectedUser = users[0];
            expectedUser.Id = 1;
            var user = await _userRepository.Find(expectedUser.Id);

            user.Should().BeEquivalentTo(expectedUser, options =>
                options.Excluding(o => o.Created));
        }

        [Test]
        public async Task UserRepository_GetCities_ReturnsCities()
        {
            var expectedUsers = await _dataProvider.GetUsers();
            var users = (await _userRepository.GetUsers()).ToList();

            users.Should().BeEquivalentTo(expectedUsers, options =>
                options.Excluding(o => o.Id).Excluding(o => o.Created));
        }

        [Test]
        public async Task UserRepository_Add_AddsUser()
        {
            var user = new User()
            {
                Name = "New User",
                Username = "New Username",
                Email = "email@email"
            };

            await _userRepository.Add(user);
            await _userRepository.SaveChanges();

            var addedUser = await _userRepository.Find(user.Id);

            addedUser.Should().BeEquivalentTo(user, options =>
                options.Excluding(o => o.Id).Excluding(o => o.Created));
        }

        [Test]
        public async Task UserRepository_Update_UpdatesUser()
        {
            var user = await _userRepository.Find(1);
            user.Name = "Updated Name";
            user.Username = "Updated Username";

            await _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var updatedUser = await _userRepository.Find(user.Id);

            updatedUser.Should().BeEquivalentTo(user, options =>
                options.Excluding(o => o.Id).Excluding(o => o.Created));
        }

        [Test]
        public async Task UserRepository_Delete_DeletesUser()
        {
            var existingUser = await _userRepository.Find(1);

            Assert.IsNotNull(existingUser);

            await _userRepository.Delete(1);
            await _userRepository.SaveChanges();

            var deletedUser = await _userRepository.Find(1);

            deletedUser.Should().BeNull();
        }

        [Test]
        public void UserRepository_Delete_WhenUserDoesNotExist_DoesNotThrowException()
        {
            Assert.DoesNotThrowAsync(async () => await _userRepository.Delete(100));
            Assert.DoesNotThrowAsync(async () => await _userRepository.SaveChanges());
        }

        [Test]
        public async Task UserRepository_GetUserByUsername_ReturnsUser()
        {
            var users = await _dataProvider.GetUsers();
            var expectedUser = users[0];
            var user = await _userRepository.GetUserByUsername(expectedUser.Username);

            user.Should().BeEquivalentTo(expectedUser, options =>
                options.Excluding(o => o.Id).Excluding(o => o.Created));
        }

        [Test]
        public async Task UserRepository_GetUserByUsername_WhenUserDoesNotExist_ReturnsNull()
        {
            var user = await _userRepository.GetUserByUsername("nonexistent");

            user.Should().BeNull();
        }
    }
}
