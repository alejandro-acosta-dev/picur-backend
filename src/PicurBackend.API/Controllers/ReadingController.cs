using Microsoft.AspNetCore.Mvc;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadingController : ControllerBase
    {
        private readonly IReadingRepository _readingRepository;

        public ReadingController(IReadingRepository readingRepository)
        {
            _readingRepository = readingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReadings()
        {
            var readings = await _readingRepository.GetAllAsync();
            return Ok(readings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReading(int id)
        {
            var user = await _readingRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(Reading reading)
        {
            var createdReading = await _readingRepository.CreateAsync(reading);

            return CreatedAtAction(nameof(GetReading), new { id = createdReading.Id }, createdReading);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _readingRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
