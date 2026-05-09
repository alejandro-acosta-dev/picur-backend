using Mapster;
using PicurBackend.Application.Common.Exceptions;
using PicurBackend.Application.Dto;
using PicurBackend.Application.Interfaces;
using PicurBackend.Domain.Entities;
using PicurBackend.Domain.Interfaces;

namespace PicurBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Adapt<IEnumerable<UserDto>>();
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("Usuario", id);
            return user.Adapt<UserDto>();
        }

        public async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new NotFoundException("Usuario", email);
            return user.Adapt<UserDto>();
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ConflictException($"Ya existe un usuario con el email '{dto.Email}'.");

            var user = dto.Adapt<User>();
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.CreateAsync(user);
            return user.Adapt<UserDto>();
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new NotFoundException("Usuario", id);

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.Phone = dto.Phone;

            var updatedUser = await _userRepository.UpdateAsync(id, existingUser);
            return updatedUser!.Adapt<UserDto>();
        }

        public async Task<UserDto> UpdatePassword(int id, string password)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("Usuario", id);

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            var updatedUser = await _userRepository.UpdateAsync(id, user);
            return updatedUser!.Adapt<UserDto>();
        }

        public async Task<UserDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new NotFoundException($"No existe un usuario con el email '{request.Email}'.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return null;

            return user.Adapt<UserDto>();
        }

        public async Task DeleteAsync(int id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                throw new NotFoundException("Usuario", id);
        }
    }
}
