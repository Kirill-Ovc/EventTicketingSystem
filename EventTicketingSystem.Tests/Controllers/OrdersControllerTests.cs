using System.Net;
using AutoFixture;
using EventTicketingSystem.API.Controllers;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace EventTicketingSystem.Tests.Controllers
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private IOrderService _orderService;
        private OrdersController _controller;
        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _orderService = Substitute.For<IOrderService>();
            _controller = new OrdersController(_orderService);
        }

        [Test]
        public async Task GetCart_WithExistingCart_ReturnsCart()
        {
            // Arrange
            var cartId = "cartId";
            var expectedCart = _fixture.Create<Cart>();
            _orderService.GetCart(cartId).Returns(expectedCart);

            // Act
            var result = await _controller.GetCart(cartId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedCart);
            await _orderService.Received(1).GetCart(cartId);
        }

        [Test]
        public async Task GetCart_WithNullCartId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCart(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID is required");
        }

        [Test]
        public async Task AddToCart_WithValidOrder_ReturnsCart()
        {
            // Arrange
            var cartId = "cartId";
            var order = _fixture.Create<SeatOrder>();
            var cart = _fixture.Create<Cart>();
            _orderService.AddToCart(cartId, order).Returns(cart);

            // Act
            var result = await _controller.AddToCart(cartId, order);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(cart);
            await _orderService.Received(1).AddToCart(cartId, order);
        }

        [Test]
        public async Task AddToCart_WithNullCartId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddToCart(null, _fixture.Create<SeatOrder>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID and Order are required");
        }

        [Test]
        public async Task DeleteSeat_WithValidParameters_ReturnsOk()
        {
            // Arrange
            var cartId = "cartId";
            var eventId = 1;
            var seatId = 1;
            var cart = _fixture.Create<Cart>();
            _orderService.RemoveFromCart(cartId, seatId).Returns(cart);

            // Act
            var result = await _controller.DeleteSeat(cartId, eventId, seatId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(cart);
            await _orderService.Received(1).RemoveFromCart(cartId, seatId);
        }

        [Test]
        public async Task DeleteSeat_WithNullCartId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteSeat(null, 1, 1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID, Event ID and Seat ID are required");
        }

        [Test]
        public async Task DeleteSeat_WithInvalidEventId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteSeat("cartId", null, 1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID, Event ID and Seat ID are required");
        }

        [Test]
        public async Task DeleteSeat_WithInvalidSeatId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteSeat("cartId", 1, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID, Event ID and Seat ID are required");
        }

        [Test]
        public async Task BookCart_WithValidCart_ReturnsPaymentId()
        {
            // Arrange
            var cartId = "cartId";
            var paymentId = 1;
            _orderService.CheckoutCart(cartId).Returns(paymentId);

            // Act
            var result = await _controller.BookCart(cartId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(paymentId);
            await _orderService.Received(1).CheckoutCart(cartId);
        }

        [Test]
        public async Task BookCart_WithNullCartId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.BookCart(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Cart ID is required");
        }

        [Test]
        public void BookCart_WithException_ReturnsInternalServerError()
        {
            // Arrange
            var cartId = "cartId";
            var errorMessage = "error message";
            _orderService.CheckoutCart(cartId).ThrowsAsync(x => throw new InvalidOperationException(errorMessage));

            // Act
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.BookCart(cartId));
        }
    }
}
