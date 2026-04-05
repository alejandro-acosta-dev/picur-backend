using PicurBackend.Domain.Entities;

namespace PicurBackend.Domain.Interfaces
{
    public interface IReadingRepository
    {
        Task<IEnumerable<Reading>> GetAllAsync();
        Task<Reading?> GetByIdAsync(int id);
        Task<Reading> CreateAsync(Reading reading);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Reading>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<double> GetAverageTemperatureAsync(DateTime start, DateTime end);
        Task<IEnumerable<Reading>> GetDoorEventsAsync(DateTime start, DateTime end);
        Task<IEnumerable<Reading>> GetAnomaliesAsync(DateTime start, DateTime end);
    }
}
