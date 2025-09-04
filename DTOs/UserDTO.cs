using System.ComponentModel.DataAnnotations;

namespace Movie_Management_API.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int BookingCount { get; set; }
        public Boolean IsActive { get; set; } = true;
    }
    public class UserAddDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public Boolean IsActive { get; set; } = true;
    }
    public class UserEditDTO
    {
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
