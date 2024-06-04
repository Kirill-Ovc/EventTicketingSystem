using EventTicketingSystem.API.Controllers;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Models.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace EventTicketingSystem.Tests.Controllers
{
    [TestFixture]
    public class PaymentsControllerTests
    {
        private IPaymentService _paymentService;
        private PaymentsController _controller;

        [SetUp]
        public void SetUp()
        {
            _paymentService = Substitute.For<IPaymentService>();
            _controller = new PaymentsController(_paymentService);
        }

        [Test]
        public async Task GetStatus_WithExistingPayment_ReturnsStatus()
        {
            // Arrange
            var paymentId = 1;
            var paymentStatus = PaymentStatus.Pending;
            var expectedStatus = "Pending";
            _paymentService.GetPaymentStatus(paymentId).Returns(paymentStatus);

            // Act
            var result = await _controller.GetStatus(paymentId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expectedStatus);
            await _paymentService.Received(1).GetPaymentStatus(paymentId);
        }

        [Test]
        public async Task GetPayment_WithNonExistingPayment_ReturnsBadRequest()
        {
            // Arrange
            var paymentId = 1;
            var errorMessage = "Payment not found";
            _paymentService.GetPaymentStatus(paymentId).Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = await _controller.GetStatus(paymentId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task PaymentCompleted_WithValidPaymentId_ReturnsOk()
        {
            // Arrange
            var paymentId = 1;
            var status = PaymentStatus.Paid;
            _paymentService.PaymentCompleted(paymentId).Returns(status);

            // Act
            var result = await _controller.Complete(paymentId);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task PaymentFailed_WithValidPaymentId_ReturnsOk()
        {
            // Arrange
            var paymentId = 1;
            var status = PaymentStatus.Cancelled;
            _paymentService.PaymentFailed(paymentId).Returns(status);

            // Act
            var result = await _controller.Failed(paymentId);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task PaymentCompleted_WithInvalidPaymentId_ReturnsBadRequest()
        {
            // Arrange
            var paymentId = 1;
            var errorMessage = "Payment not found";
            _paymentService.PaymentCompleted(paymentId).Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = await _controller.Complete(paymentId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task PaymentFailed_WithInvalidPaymentId_ReturnsBadRequest()
        {
            // Arrange
            var paymentId = 1;
            var errorMessage = "Payment not found";
            _paymentService.PaymentFailed(paymentId).Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = await _controller.Failed(paymentId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
