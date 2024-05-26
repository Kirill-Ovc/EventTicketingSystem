using EventTicketingSystem.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        [HttpGet("{city_id}")]
        public async Task<IActionResult> GetByCity(int cityId)
        {
            if (cityId == default)
            {
                return BadRequest("City ID is required");
            }
            var events = await _eventService.GetEventsByCity(cityId);
            return Ok(events);
        }

        [HttpGet("events/{event_id}/sections/{section_id}/seats")]
        public async Task<IActionResult> GetSeats(int eventId, int sectionId)
        {
            if (eventId == default || sectionId == default)
            {
                return BadRequest("Event ID and Section ID are required");
            }
            var seats = await _eventService.GetEventSeats(eventId, sectionId);
            return Ok(seats);
        }
    }
}
