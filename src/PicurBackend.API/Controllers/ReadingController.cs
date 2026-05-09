using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Entities;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadingController : ControllerBase
    {
        private readonly IReadingService _readingService;

        public ReadingController(IReadingService readingService)
        {
            _readingService = readingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReadings()
        {
            var readings = await _readingService.GetAllAsync();
            return Ok(readings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReading(int id)
        {
            var reading = await _readingService.GetByIdAsync(id);
            return Ok(reading);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReading([FromBody] Reading reading)
        {
            var createdReading = await _readingService.CreateAsync(reading);
            return CreatedAtAction(nameof(GetReading), new { id = createdReading.Id }, createdReading);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReading(int id)
        {
            await _readingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
