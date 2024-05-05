using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository: IRepository<User>
    {
        Task<ICollection<User>> GetUsers();
        Task<User> GetUserByUsername(string username);
    }
}
