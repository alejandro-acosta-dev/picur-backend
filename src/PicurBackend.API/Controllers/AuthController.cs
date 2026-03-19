using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Dto;
using PicurBackend.Application.Interfaces;
using PicurBackend.Application.Services;

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


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _userService.LoginAsync(dto);

            return result ? Ok() : BadRequest();            
        }
    }
}