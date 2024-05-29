using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("carts/{cartId}")]
        public async Task<ActionResult<Cart>> GetCart(string cartId)
        {
            var cart = await _orderService.GetCart(cartId);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPost("carts/{cartId}")]
        public async Task<ActionResult<Cart>> AddToCart(string cartId, SeatOrder order)
        {
            try
            {
                var cart = await _orderService.AddToCart(cartId, order);
                if (cart == null)
                {
                    return NotFound();
                }
                return Ok(cart);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        public async Task<ActionResult<Cart>> DeleteSeat(string cartId, int eventId, int seatId)
        {
            var cart = await _orderService.RemoveFromCart(cartId, seatId);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPut("carts/{cartId}/book")]
        public async Task<ActionResult<int>> BookCart(string cartId)
        {
            try
            {
                var paymentId = await _orderService.CheckoutCart(cartId);
                return Ok(paymentId);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
