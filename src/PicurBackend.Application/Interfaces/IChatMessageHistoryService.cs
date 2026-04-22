using PicurBackend.Application.Dto;

namespace PicurBackend.Application.Interfaces
{
    public interface IChatMessageHistoryService
    {
        Task<IEnumerable<ChatMessageHistoryDto>> GetHistory();
        Task CleanHistory();
    }
}
