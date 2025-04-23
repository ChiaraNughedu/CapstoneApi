using ApiVille.DTOs;
using ApiVille.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiVille.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var (success, errorMessage) = await _authService.RegisterAsync(model);
            if (!success)
                return BadRequest(errorMessage);

            return Ok("Registrazione avvenuta con successo");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var (success, errorMessage, tokenData) = await _authService.LoginAsync(model);
            if (!success)
                return Unauthorized(errorMessage);

            return Ok(tokenData);
        }
    }
}
