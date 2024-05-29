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
        public async Task<IActionResult> GetStatus(int paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentStatus(paymentId);
                return Ok(payment.ToString());
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<IActionResult> Complete(int paymentId)
        {
            try
            {
                await _paymentService.PaymentCompleted(paymentId);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{paymentId}/failed")]
        public async Task<IActionResult> Failed(int paymentId)
        {
            try
            {
                await _paymentService.PaymentFailed(paymentId);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
