using CQRSapp.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRSapp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthenticationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _sender.Send(command);
            if (result == null)
            {
                return Unauthorized(); // Return the token or user info
            }
            
            return Ok(result); // Return the token or user info)

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _sender.Send(command);
            if (result)
            {
                return Ok("User registered successfully.");
            }
            else
            {
                return BadRequest("Failed to register user.");
            }
        }
    }
}
