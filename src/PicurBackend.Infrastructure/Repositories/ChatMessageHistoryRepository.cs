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
