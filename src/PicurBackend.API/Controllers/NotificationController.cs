using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Services;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendSms()
        {
            var code = await _notificationService.SendSmsAsync();

            return Ok(new
            {
                Message = "Código enviado correctamente, por favor ingresarlo para recuperar su contraseña",
                code
            });
        }
    }
}