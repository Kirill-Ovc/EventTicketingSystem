﻿namespace EventTicketingSystem.DataAccess.Models.DTOs
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
