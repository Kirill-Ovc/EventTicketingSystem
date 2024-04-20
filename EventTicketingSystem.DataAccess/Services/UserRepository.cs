using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class UserRepository : IUserRepository
    {
        public async Task<ICollection<User>> GetUsers()
        {
            //Test
            return new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Name = "User1"
                }
            };
        }
    }
}
