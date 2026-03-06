using PicurBackend.Domain.Entities;

namespace PicurBackend.Domain.Interfaces
{
    public interface IReadingRepository
    {
        Task<IEnumerable<Reading>> GetAllAsync();

        Task<Reading?> GetByIdAsync(int id);

        Task<Reading> CreateAsync(Reading reading);

        Task<bool> DeleteAsync(int id);
    }
}
