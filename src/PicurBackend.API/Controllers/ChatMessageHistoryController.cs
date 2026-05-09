using Microsoft.AspNetCore.Mvc;
using PicurBackend.Application.Interfaces;

namespace PicurBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessageHistoryController : ControllerBase
    {
        private readonly IChatMessageHistoryService _chatMessageHistoryService;

        public ChatMessageHistoryController(IChatMessageHistoryService chatMessageHistoryService)
        {
            _chatMessageHistoryService = chatMessageHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _chatMessageHistoryService.GetHistory();
            return Ok(history);
        }

        [HttpDelete]
        public async Task<IActionResult> CleanHistory()
        {
            await _chatMessageHistoryService.CleanHistory();
            return NoContent();
        }
    }
}
