using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Interfaces;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAIService _aiService;

        public OpenAIController(IOpenAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost]
        public async Task<IActionResult> AskAI([FromBody] string prompt)
        {
            var result = await _aiService.AskAI(prompt);
            return Ok(new { response = result });
        }
    }
}
