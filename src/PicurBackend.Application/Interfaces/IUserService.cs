using PicurBackend.Application.Dto;

namespace PicurBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<UserDto> UpdatePassword(int id, string password);
        Task<UserDto?> LoginAsync(LoginRequestDto request);
        Task DeleteAsync(int id);
    }
}
