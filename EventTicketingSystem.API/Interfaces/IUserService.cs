using EventTicketingSystem.DataAccess.Models.DTOs;

namespace EventTicketingSystem.API.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<UserInfoDto>> GetUsers();

        Task<UserInfoDto> CreateUser(UserInfoDto user);
    }
}
