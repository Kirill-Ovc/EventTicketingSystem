using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventTicketingSystem.API.Models;
using Microsoft.Extensions.Options;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly SymmetricSecurityKey _key;
        private readonly TimeSpan _tokenLifetime;
        private readonly string _issuer;
        private readonly string _audience;

        public IdentityController(IOptions<JwtSettings> options)
        {
            var tokenKey = options.Value.SecretKey;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _tokenLifetime = TimeSpan.FromHours(options.Value.TokenExpirationHours);
        }

        [HttpPost("token")]
        public IActionResult GenerateToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, "test"),
                new(JwtRegisteredClaimNames.Email, "")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_tokenLifetime),
                SigningCredentials = creds,
                Issuer = _issuer,
                Audience = _audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(jwt);
        }
    }
}
