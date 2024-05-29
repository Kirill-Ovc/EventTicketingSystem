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
            var cart = _fixture.Create<Cart>();
            _orderService.GetCart(cartId).Returns(cart);

            // Act
            var response = await _controller.GetCart(cartId);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(cart);
            await _orderService.Received(1).GetCart(cartId);
        }

        [Test]
        public async Task GetCart_WithNonExistingCart_ReturnsNotFound()
        {
            // Arrange
            var cartId = "cartId";
            _orderService.GetCart(cartId).Returns((Cart)null);

            // Act
            var result = await _controller.GetCart(cartId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
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
            var response = await _controller.AddToCart(cartId, order);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(cart);
            await _orderService.Received(1).AddToCart(cartId, order);
        }

        [Test]
        public async Task AddToCart_WithNonExistingCart_ReturnsNotFound()
        {
            // Arrange
            var cartId = "cartId";
            var order = _fixture.Create<SeatOrder>();
            _orderService.AddToCart(cartId, order).Returns((Cart)null);

            // Act
            var result = await _controller.AddToCart(cartId, order);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task AddToCart_WithInvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            var cartId = "cartId";
            var order = _fixture.Create<SeatOrder>();
            var errorMessage = "error message";
            _orderService.AddToCart(cartId, order).Throws(new InvalidOperationException(errorMessage));

            // Act
            var response = await _controller.AddToCart(cartId, order);
            var result = response.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(errorMessage);
            await _orderService.Received(1).AddToCart(cartId, order);
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
            var response = await _controller.DeleteSeat(cartId, eventId, seatId);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(cart);
            await _orderService.Received(1).RemoveFromCart(cartId, seatId);
        }

        [Test]
        public async Task DeleteSeat_WithNonExistingCart_ReturnsNotFound()
        {
            // Arrange
            var cartId = "cartId";
            var eventId = 1;
            var seatId = 1;
            _orderService.RemoveFromCart(cartId, seatId).Returns((Cart)null);

            // Act
            var result = await _controller.DeleteSeat(cartId, eventId, seatId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        //BookCart
        public async Task BookCart_WithValidCart_ReturnsPaymentId()
        {
            // Arrange
            var cartId = "cartId";
            var paymentId = 1;
            _orderService.CheckoutCart(cartId).Returns(paymentId);

            // Act
            var response = await _controller.BookCart(cartId);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(paymentId);
            await _orderService.Received(1).CheckoutCart(cartId);
        }

        [Test]
        //BookCart checkout throws exception
        public async Task BookCart_WithException_ReturnsInternalServerError()
        {
            // Arrange
            var cartId = "cartId";
            var errorMessage = "error message";
            _orderService.CheckoutCart(cartId).ThrowsAsync(x => throw new InvalidOperationException(errorMessage));

            // Act
            var response = await _controller.BookCart(cartId);
            var result = response.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.Value.Should().Be(errorMessage);
            await _orderService.Received(1).CheckoutCart(cartId);
        }
    }
}
