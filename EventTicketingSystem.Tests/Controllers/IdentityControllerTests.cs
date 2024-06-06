using EventTicketingSystem.API.Controllers;
using EventTicketingSystem.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EventTicketingSystem.Tests.Controllers
{
    [TestFixture]
    public class IdentityControllerTests
    {
        private IdentityController _identityService;

        [SetUp]
        public void SetUp()
        {
            var settings = new JwtSettings
            {
                SecretKey = "SecretKeySecretKeySecretKeySecretKey",
                Issuer = "Issuer",
                Audience = "Audience",
                TokenExpirationHours = 1
            };
            var options = Options.Create(settings);
            _identityService = new IdentityController(options);
        }

        [Test]
        public void GenerateToken_ReturnsToken()
        {
            // Act
            var result = _identityService.GenerateToken();

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<string>()
                .And.NotBeNull();
        }

    }
}
