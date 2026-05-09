using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Dto;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensoresController : ControllerBase
    {
        private readonly ILogger<SensoresController> _logger;

        public SensoresController(ILogger<SensoresController> logger)
        {
            _logger = logger;
        }

        [HttpPost("datos")]
        public IActionResult RecibirDatos([FromBody] SensorDto sensorDto)
        {
            _logger.LogInformation(
                "Datos recibidos del ESP32 - Temperatura: {Temperatura} °C, PuertaAbierta: {PuertaAbierta}",
                sensorDto.Temperatura,
                sensorDto.PuertaAbierta);

            return Ok();
        }
    }
}
