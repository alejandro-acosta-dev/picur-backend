using PicurBackend.Domain.Entities;

namespace PicurBackend.Domain.Interfaces
{
    public interface IChatMessageHistoryRepository
    {
        Task<List<ChatMessageHistory>> GetHistoryAsync();
        Task SaveMessageAsync(ChatMessageHistory message);
    }
}
