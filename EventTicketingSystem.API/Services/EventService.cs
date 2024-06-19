using AutoMapper;
using EventTicketingSystem.API.Constants;
using EventTicketingSystem.API.Exceptions;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace EventTicketingSystem.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        public EventService(
            IEventRepository eventRepository,
            ISeatRepository seatRepository, 
            IOfferRepository offerRepository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _eventRepository = eventRepository;
            _seatRepository = seatRepository;
            _offerRepository = offerRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IList<EventDto>> GetAllEvents()
        {
            return await _cache.GetOrCreate(CacheKeys.Events, async entry =>
            {
                entry.SetOptions(_cacheOptions);
                return await GetAllEventsInternal();
            });
        }

        private async Task<IList<EventDto>> GetAllEventsInternal()
        {
            var events = await _eventRepository.GetEvents();
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<IList<EventDto>> GetEventsByCity(int cityId)
        {
            var cacheKey = string.Format(CacheKeys.CityEvents, cityId);
            return await _cache.GetOrCreate(string.Format(cacheKey, cityId), async entry =>
            {
                entry.SetOptions(_cacheOptions);
                return await GetEventsByCityInternal(cityId);
            });
        }

        private async Task<IList<EventDto>> GetEventsByCityInternal(int cityId)
        {
            var events = await _eventRepository.GetEventsByCity(cityId);
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<IList<EventSeatDto>> GetEventSeats(int eventId, int sectionId)
        {
            var seats = await GetCachedEventSeats(eventId);
            var offers = await _offerRepository.GetOffersByEvent(eventId);
            var sectionOffers = offers.Where(o => o.SectionId == sectionId).ToList();

            var eventSeats = seats
                .Where(s => s.Seat.SectionId == sectionId)
                .Select(s => MapToEventSeatDto(s, sectionOffers)).ToList();

            return eventSeats;
        }

        private async Task<ICollection<EventSeat>> GetCachedEventSeats(int eventId)
        {
            var cacheKey = string.Format(CacheKeys.EventSeats, eventId);
            if (!_cache.TryGetValue(cacheKey, out ICollection<EventSeat> eventSeats))
            {
                var checkEvent = await _eventRepository.Find(eventId);
                if (checkEvent is null)
                {
                    throw new EntityNotFoundException("Event not found");
                }
                eventSeats = await _seatRepository.GetEventSeats(eventId);
                _cache.Set(cacheKey, eventSeats, _cacheOptions);
            }

            return eventSeats;
        }

        private EventSeatDto MapToEventSeatDto(EventSeat seat, List<Offer> offers)
        {
            var eventSeat = _mapper.Map<EventSeatDto>(seat);
            eventSeat.Prices = offers.Where(o => o.RowNumber == seat.Seat!.RowNumber)
                .Select(o => new SeatOfferDto()
                {
                    OfferId = o.Id,
                    Price = o.Price,
                    TicketLevel = o.TicketLevel
                }).ToList();

            return eventSeat;
        }
    }
}
