using AutoMapper;
using EventTicketingSystem.API.Helpers;
using EventTicketingSystem.API.Services;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;
using FluentAssertions;
using NSubstitute;

namespace EventTicketingSystem.Tests.Services
{
    public class UserServiceTests
    {
        private UserService _userService;
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            var mapperProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            _mapper = new Mapper(configuration);
        }

        [SetUp]
        public void Setup()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _userService = new UserService(_userRepository, _mapper);
        }

        [Test]
        public async Task UserService_GetUsers_ReturnsUsers()
        {
            // Arrange
            _userRepository.GetUsers().Returns(_testUsers);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            result.Should().BeEquivalentTo(_testUserInfos);
        }

        [Test]
        public async Task UserService_CreateUser_ReturnsUser()
        {
            // Arrange
            var newUser = new UserInfoDto()
            {
                Id = 1,
                Username = "testuser1",
                Name = "Test User 1",
                Email = "user1@email.net"
            };
            var expectedUser = _testUsers[0];

            // Act
            var result = await _userService.CreateUser(newUser);

            // Assert
            result.Should().BeEquivalentTo(newUser);
            await _userRepository.Received(1).Add(Arg.Is<User>(a => a.Username == expectedUser.Username));
            await _userRepository.Received(1).SaveChanges();
        }


        private readonly List<User> _testUsers = new List<User>()
        {
            new User()
            {
                Id = 1,
                Username = "testuser1",
                Password = "password",
                Name = "Test User 1",
                Email = "user1@email.net"
            },
            new User()
            {
                Id = 2,
                Username = "testuser2",
                Password = "password",
                Name = "Test User 2",
                Email = "user2@email.net"
            }
        };

        private readonly List<UserInfoDto> _testUserInfos = new List<UserInfoDto>()
        {
            new UserInfoDto()
            {
                Id = 1,
                Username = "testuser1",
                Name = "Test User 1",
                Email = "user1@email.net"
            },
            new UserInfoDto()
            {
                Id = 2,
                Username = "testuser2",
                Name = "Test User 2",
                Email = "user2@email.net"
            }
        };
    }
}
