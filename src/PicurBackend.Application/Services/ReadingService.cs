using PicurBackend.Application.Common.Exceptions;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Application.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IReadingRepository _readingRepository;

        public ReadingService(IReadingRepository readingRepository)
        {
            _readingRepository = readingRepository;
        }

        public async Task<IEnumerable<Reading>> GetAllAsync()
        {
            return await _readingRepository.GetAllAsync();
        }

        public async Task<Reading> GetByIdAsync(int id)
        {
            var reading = await _readingRepository.GetByIdAsync(id);
            if (reading == null)
                throw new NotFoundException("Lectura", id);
            return reading;
        }

        public async Task<Reading> CreateAsync(Reading reading)
        {
            return await _readingRepository.CreateAsync(reading);
        }

        public async Task DeleteAsync(int id)
        {
            var deleted = await _readingRepository.DeleteAsync(id);
            if (!deleted)
                throw new NotFoundException("Lectura", id);
        }
    }
}
