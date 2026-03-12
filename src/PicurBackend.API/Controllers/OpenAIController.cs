using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Services;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenAIController : ControllerBase
    {
        private readonly OpenAIService _aiService;

        public OpenAIController( OpenAIService aiService)
        {
            _aiService = aiService;
        }


        [HttpPost]
        public async Task<IActionResult> AskAI([FromBody] string prompt)
        {
            var result = await _aiService.AskAI(prompt);

            return Ok(result);
        }
    }
}