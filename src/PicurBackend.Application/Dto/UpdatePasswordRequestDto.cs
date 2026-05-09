using System.ComponentModel.DataAnnotations;

namespace PicurBackend.Application.Dto
{
    public class UpdatePasswordRequestDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
