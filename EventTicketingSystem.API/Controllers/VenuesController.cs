using EventTicketingSystem.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ISeatRepository _seatRepository;

        public VenuesController(
            IVenueRepository venueRepository,
            ISeatRepository seatRepository)
        {
            _venueRepository = venueRepository;
            _seatRepository = seatRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _venueRepository.GetVenues();
            return Ok(users);
        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetByCity(int? cityId)
        {
            if (cityId is null)
            {
                return BadRequest("City ID is required");
            }

            var venue = await _venueRepository.GetVenuesByCity(cityId.Value);
            return Ok(venue);
        }

        [HttpGet("{venueId}/sections")]
        public async Task<IActionResult> GetSections(int? venueId)
        {
            if (venueId is null)
            {
                return BadRequest("Venue ID is required");
            }

            var venue = await _seatRepository.GetSections(venueId.Value);
            return Ok(venue);
        }

    }
}
