using EventTicketingSystem.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetStatus(int? paymentId)
        {
            if (paymentId is null)
            {
                return BadRequest("Payment ID is required");
            }

            try
            {
                var paymentStatus = await _paymentService.GetPaymentStatus(paymentId.Value);
                return Ok(paymentStatus.ToString());
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<IActionResult> Complete(int? paymentId)
        {
            if (paymentId is null)
            {
                return BadRequest("Payment ID is required");
            }

            try
            {
                await _paymentService.PaymentCompleted(paymentId.Value);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{paymentId}/failed")]
        public async Task<IActionResult> Failed(int? paymentId)
        {
            if (paymentId is null)
            {
                return BadRequest("Payment ID is required");
            }

            try
            {
                await _paymentService.PaymentFailed(paymentId.Value);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
