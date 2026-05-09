using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Interfaces;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSms()
        {
            await _notificationService.SendSmsAsync();
            return Ok(new { message = "Código enviado correctamente. Por favor ingrésalo para recuperar tu contraseña." });
        }
    }
}
