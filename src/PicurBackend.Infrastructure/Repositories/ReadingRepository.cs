using Microsoft.EntityFrameworkCore;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Infrastructure.Repositories
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly ApplicationDbContext _context;

        public ReadingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reading>> GetAllAsync()
        {
            return await _context.Readings
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Reading?> GetByIdAsync(int id)
        {
            return await _context.Readings.FindAsync(id);
        }

        public async Task<Reading> CreateAsync(Reading reading)
        {
            _context.Readings.Add(reading);
            await _context.SaveChangesAsync();

            return reading;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reading = await _context.Readings.FindAsync(id);

            if (reading == null)
                return false;

            _context.Readings.Remove(reading);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Reading>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Readings
               .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate)
               .OrderBy(r => r.Timestamp)
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<double> GetAverageTemperatureAsync(DateTime start, DateTime end)
        {
            var avg = await _context.Readings
                .Where(r => r.Timestamp >= start && r.Timestamp <= end)
                .Select(r => (double?)r.Temperature)
                .AverageAsync();

            return avg ?? 0;
        }

        public async Task<IEnumerable<Reading>> GetDoorEventsAsync(DateTime start, DateTime end)
        {
            return await _context.Readings
                .Where(r => r.Timestamp >= start &&
                            r.Timestamp <= end &&
                            r.Door == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Reading>> GetAnomaliesAsync(DateTime start, DateTime end)
        {
            return await _context.Readings
                .Where(r =>
                    r.Timestamp >= start &&
                    r.Timestamp <= end &&
                    (r.Temperature < 2 || r.Temperature > 8))
                .OrderBy(r => r.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
