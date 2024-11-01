using FeedBack_System.Models;
using FeedBack_System.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FeedBack_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var authResponse = await _authService.AuthenticateAsync(loginRequest);

            if (authResponse == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(authResponse);
        }
    }
}
