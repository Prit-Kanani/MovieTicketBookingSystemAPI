namespace Movie_Management_API.Models
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
