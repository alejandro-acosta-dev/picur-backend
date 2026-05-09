using System.ComponentModel.DataAnnotations;

namespace PicurBackend.Application.Dto
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}
