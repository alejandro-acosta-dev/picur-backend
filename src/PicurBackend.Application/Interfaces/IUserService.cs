using PicurBackend.Application.Dto;
using PicurBackend.Domain.Entities;

namespace PicurBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<UserDto?> CreateAsync(User user);
        Task<UserDto> UpdatePassword(int id, string password);
        Task<bool> LoginAsync(LoginRequestDto request);
        Task<UserDto?> GetUserByEmail(string email); 
    }
}
