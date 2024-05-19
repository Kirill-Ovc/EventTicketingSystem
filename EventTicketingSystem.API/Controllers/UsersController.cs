using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.DataAccess.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserInfoDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            await _userService.CreateUser(user);
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
    }
}
