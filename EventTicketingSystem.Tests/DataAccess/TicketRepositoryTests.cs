using AutoMapper;
using EventTicketingSystem.DataAccess.Helpers;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.DTOs;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class TicketRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private ITicketRepository _ticketRepository;
        private DatabaseContext _context;

        public TicketRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();
            _ticketRepository = new TicketRepository(_context, mapper);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedUsers();
            await seeder.SeedVenues();
            await seeder.SeedSeats(1);
            await seeder.SeedEvents();
            await seeder.SeedEventSeats(1, 1);
            await seeder.SeedTickets();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task TicketRepository_GetTickets_ReturnsTickets()
        {
            var expectedTicket = new TicketDto()
            {
                Id = 1,
                UserId = 1,
                Price = 1000,
                Status = TicketStatus.Active,
                TicketLevel = TicketLevel.Adult,
                EventName = "Concert of famous artist",
                EventDateTime = "01-May-24 4:00:00 PM",
                EventVenue = "Tinkoff Arena",
                SeatNumber = "A1-11",
                RowNumber = "1",
                SectionName = "Section A"
            };

            var tickets = await _ticketRepository.GetTickets(expectedTicket.UserId);

            tickets.Should().ContainSingle();
            tickets[0].Should().BeEquivalentTo(expectedTicket);
        }
    }
}
