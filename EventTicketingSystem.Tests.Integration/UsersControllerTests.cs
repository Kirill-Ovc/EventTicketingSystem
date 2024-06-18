using System.Net;
using EventTicketingSystem.DataAccess.Models.DTOs;
using FluentAssertions;

namespace EventTicketingSystem.Tests.Integration
{
    public class UsersControllerTests
    {
        private readonly HttpClient _client;

        public UsersControllerTests()
        {
            var app = new TestWebApplication();
            app.AuthenticateAsync().Wait();
            _client = app.Client;
        }

        [Test]
        public async Task GetUsers()
        {
            // Arrange
            var usersGet = "api/users";

            // Act
            var response = _client.GetAsync(usersGet).Result;
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<List<UserInfoDto>>();
            result.Should().BeEquivalentTo(_testUsers);
        }

        private readonly List<UserInfoDto> _testUsers = new()
        {
            new()
            {
                Id = 1,
                Username = "admin",
                Name = "Admin",
                Email = "admin@email"
            },
            new()
            {
                Id = 2,
                Username = "TestUser",
                Name = "Test User",
                Email = "test@email"
            },
            new()
            {
                Id = 3,
                Username = "Kirill1",
                Name = "Kirill O",
                Email = "kirill@email"
            }
        };
    }
}