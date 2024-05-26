using AutoMapper;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepository,
            ISeatRepository seatRepository, 
            IOfferRepository offerRepository,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _seatRepository = seatRepository;
            _offerRepository = offerRepository;
            _mapper = mapper;
        }

        public async Task<IList<EventDto>> GetAllEvents()
        {
            var events = await _eventRepository.GetEvents();
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<IList<EventDto>> GetEventsByCity(int cityId)
        {
            var events = await _eventRepository.GetEventsByCity(cityId);
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<IList<EventSeatDto>> GetEventSeats(int eventId, int sectionId)
        {
            var seats = await _seatRepository.GetEventSeats(eventId, sectionId);
            var offers = await _offerRepository.GetOffersByEvent(eventId);
            var sectionOffers = offers.Where(o => o.SectionId == sectionId).ToList();

            var eventSeats = seats.Select(s => new EventSeatDto
            {
                Id = s.Id,
                Name = s.Name,
                EventId = eventId,
                SectionId = sectionId,
                RowNumber = s.Seat!.RowNumber,
                SeatId = s.SeatId,
                Status = s.Status,
                Prices = sectionOffers.Where(o => o.RowNumber == s.Seat.RowNumber)
                    .Select(o => new SeatOfferDto()
                    {
                        OfferId = o.Id,
                        Price = o.Price,
                        TicketLevel = o.TicketLevel
                    }).ToList()
            }).ToList();

            return eventSeats;
        }
    }
}
