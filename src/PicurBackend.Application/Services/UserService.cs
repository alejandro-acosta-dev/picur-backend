
using Mapster;
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

        public async Task<UserDto> CreateAsync(User user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            user.Password = hashedPassword;

            var createdUser = await _userRepository.CreateAsync(user);

            UserDto dto = user.Adapt<UserDto>();

            return dto;
        }

        public async Task<bool> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        }


        public async Task<UserDto> UpdatePassword(int id, string password)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            user.Password = hashedPassword;

            var updatedUser = await _userRepository.UpdateAsync(id, user);

            UserDto dto = updatedUser.Adapt<UserDto>();
            return dto;

        }


    }
}
