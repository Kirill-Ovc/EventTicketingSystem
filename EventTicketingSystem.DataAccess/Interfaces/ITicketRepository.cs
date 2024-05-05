using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;

namespace EventTicketingSystem.DataAccess.Interfaces;

internal interface ITicketRepository: IRepository<Ticket>
{
    Task<List<TicketDto>> GetTickets(int userId);
}