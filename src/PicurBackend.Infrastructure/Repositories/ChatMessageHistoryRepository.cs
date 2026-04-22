using Microsoft.EntityFrameworkCore;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Infrastructure.Repositories
{
    public class ChatMessageHistoryRepository : IChatMessageHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CleanHistory()
        {
            var tableName = _context.Model.FindEntityType(typeof(ChatMessageHistory)).GetTableName();
            _context.Database.ExecuteSqlRaw($"TRUNCATE TABLE [ChatMessagesHistory]");
        }

        public async Task<IEnumerable<ChatMessageHistory>> GetAllHistoryAsync()
        {
            return await _context.ChatMessageHistories.AsNoTracking().ToListAsync();
        }

        public async Task<List<ChatMessageHistory>> GetHistoryAsync()
        {
            return await _context.ChatMessageHistories.AsNoTracking().Take(10).ToListAsync();
        }

        public async Task SaveMessageAsync(ChatMessageHistory message)
        {
            _context.ChatMessageHistories.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
