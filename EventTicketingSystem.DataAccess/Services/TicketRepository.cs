using AutoMapper;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class TicketRepository : ITicketRepository
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public TicketRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddTicket(Ticket ticket)
        {
            await _context.AddAsync(ticket);
        }

        public async Task<List<TicketDto>> GetTickets(int userId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.EventSeat).ThenInclude(es => es.Event)
                .Include(t => t.EventSeat).ThenInclude(es => es.Seat)
                .ThenInclude(s => s.Section)
                .Where(t => t.UserId == userId)
                .ToListAsync();
            var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);
            return ticketDtos;
        }

    }
}
