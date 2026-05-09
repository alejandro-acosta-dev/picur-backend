using PicurBackend.Domain.Entities;

namespace PicurBackend.Application.Interfaces
{
    public interface IReadingService
    {
        Task<IEnumerable<Reading>> GetAllAsync();
        Task<Reading> GetByIdAsync(int id);
        Task<Reading> CreateAsync(Reading reading);
        Task DeleteAsync(int id);
    }
}
