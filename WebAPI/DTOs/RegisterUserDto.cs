using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public int WorkGroupId { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
