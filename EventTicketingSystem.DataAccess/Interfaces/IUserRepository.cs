using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        public Task<ICollection<User>> GetUsers();
    }
}
