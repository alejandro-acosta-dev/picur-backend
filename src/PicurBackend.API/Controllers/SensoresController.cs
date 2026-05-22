using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Dto;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Entities;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensoresController : ControllerBase
    {
        private readonly ILogger<SensoresController> _logger;
        private readonly IReadingService _readingService;

        public SensoresController(ILogger<SensoresController> logger, IReadingService readingService)
        {
            _logger = logger;
            _readingService = readingService;
        }

        [HttpPost("datos")]
        public async Task<IActionResult> RecibirDatos([FromBody] SensorDto sensorDto)
        {
            _logger.LogInformation(
                "Datos recibidos del ESP32 - Temperatura: {Temperatura} °C, PuertaAbierta: {PuertaAbierta}",
                sensorDto.Temperatura,
                sensorDto.PuertaAbierta);

            var reading = new Reading
            {
                Temperature = sensorDto.Temperatura,
                Door = sensorDto.PuertaAbierta,
                Timestamp = DateTime.UtcNow
            };

            await _readingService.CreateAsync(reading);

            return Ok();
        }
    }
}
