using EventTicketingSystem.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> TestSending([FromBody] string operationName)
        {
            if (string.IsNullOrWhiteSpace(operationName))
            {
                return BadRequest("Empty operation name");
            }

            await _notificationService.SendNotificationTest(operationName);
            return Ok(operationName);
        }
    }
}
