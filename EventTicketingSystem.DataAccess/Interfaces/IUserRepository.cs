using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsers();
        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string username);
        Task Add(User user);
        void Update(User user);
        Task Delete(int id);
        Task SaveChanges();
    }
}
