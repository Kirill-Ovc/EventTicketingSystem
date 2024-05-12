using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.DataAccess.Models.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
