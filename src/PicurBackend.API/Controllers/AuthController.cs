using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Dto;
using PicurBackend.Application.Interfaces;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _userService.LoginAsync(dto);

            if (user == null)
                return Unauthorized(new { message = "Credenciales incorrectas." });

            return Ok(user);
        }
    }
}
