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
        public async Task<ActionResult<string>> GetStatus(int paymentId)
        {
            try
            {
                var paymentStatus = await _paymentService.GetPaymentStatus(paymentId);
                return Ok(paymentStatus.ToString());
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<ActionResult> Complete(int paymentId)
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
        public async Task<ActionResult> Failed(int paymentId)
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
