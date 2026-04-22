using Mapster;
using PicurBackend.Application.Dto;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Application.Services
{
    public class ChatMessageHistoryService : IChatMessageHistoryService
    {
        private readonly IChatMessageHistoryRepository _chatMessageHistoryRepository;
        public ChatMessageHistoryService(IChatMessageHistoryRepository chatMessageHistoryRepository)
        {
            _chatMessageHistoryRepository = chatMessageHistoryRepository;
        }

        public async Task CleanHistory()
        {
            await _chatMessageHistoryRepository.CleanHistory();
        }

        public async Task<IEnumerable<ChatMessageHistoryDto>> GetHistory()
        {
            var chatsHistory = await _chatMessageHistoryRepository.GetAllHistoryAsync();
            return chatsHistory.Adapt<IEnumerable<ChatMessageHistoryDto>>();
        }
    }
}
