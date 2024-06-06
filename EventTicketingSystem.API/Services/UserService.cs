using AutoMapper;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ICollection<UserInfoDto>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var usersResult = _mapper.Map<List<UserInfoDto>>(users);
            return usersResult;
        }

        public async Task<UserInfoDto> CreateUser(UserInfoDto user)
        {
            var newUser  = _mapper.Map<User>(user);
            newUser.Password = "password";
            await _userRepository.Add(newUser);
            await _userRepository.SaveChanges();
            user.Id = newUser.Id;
            return user;
        }
    }
}
