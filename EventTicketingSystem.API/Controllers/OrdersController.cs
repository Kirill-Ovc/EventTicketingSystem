using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetCart(string cartId)
        {
            if (cartId is null)
            {
                return BadRequest("Cart ID is required");
            }

            var cart = await _orderService.GetCart(cartId);

            return Ok(cart);
        }

        [HttpPost("carts/{cartId}")]
        public async Task<IActionResult> AddToCart(string cartId, SeatOrder order)
        {
            if (cartId is null || order is null)
            {
                return BadRequest("Cart ID and Order are required");
            }

            var cart = await _orderService.AddToCart(cartId, order);

            return Ok(cart);
        }

        [HttpDelete("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        public async Task<IActionResult> DeleteSeat(string cartId, int? eventId, int? seatId)
        {
            if (cartId is null || eventId is null || seatId is null)
            {
                return BadRequest("Cart ID, Event ID and Seat ID are required");
            }

            var cart = await _orderService.RemoveFromCart(cartId, seatId.Value);

            return Ok(cart);
        }

        [HttpPut("carts/{cartId}/book")]
        public async Task<IActionResult> BookCart(string cartId)
        {
            if (cartId is null)
            {
                return BadRequest("Cart ID is required");
            }

            var paymentId = await _orderService.CheckoutCart(cartId);

            return Ok(paymentId);
        }
    }
}
