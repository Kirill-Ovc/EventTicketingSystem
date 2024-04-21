using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface ITicketRepository
{
    Task AddTicket(Ticket ticket);
    Task<List<TicketDto>> GetTickets(int userId);
}