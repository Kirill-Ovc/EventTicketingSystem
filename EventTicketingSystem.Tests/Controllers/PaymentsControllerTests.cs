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
        public async Task GetStatus_ForExistingPayment_ReturnsStatus()
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
        public void GetStatus_ForNonExistingPayment_ThrowsError()
        {
            // Arrange
            var paymentId = 1;
            var errorMessage = "Payment not found";
            _paymentService.GetPaymentStatus(paymentId).Throws(new InvalidOperationException(errorMessage));

            // Act
            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetStatus(paymentId));
        }

        [Test]
        public async Task GetStatus_WithNullPaymentId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStatus(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Payment ID is required");
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
        public async Task PaymentCompleted_WithNullPaymentId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Complete(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Payment ID is required");
        }

        [Test]
        public async Task PaymentFailed_WithNullPaymentId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Failed(null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Payment ID is required");
        }
    }
}
