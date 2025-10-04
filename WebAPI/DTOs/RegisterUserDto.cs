using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public int WorkGroupId { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
