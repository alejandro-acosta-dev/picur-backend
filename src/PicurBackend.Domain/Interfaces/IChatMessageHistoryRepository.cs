using PicurBackend.Domain.Entities;

namespace PicurBackend.Domain.Interfaces
{
    public interface IChatMessageHistoryRepository
    {
        Task<IEnumerable<ChatMessageHistory>> GetAllHistoryAsync();
        Task<List<ChatMessageHistory>> GetHistoryAsync();
        Task SaveMessageAsync(ChatMessageHistory message);
        Task CleanHistory();
    }
}
