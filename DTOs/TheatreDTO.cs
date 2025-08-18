namespace Movie_Management_API.DTOs
{
    public class TheatreDTO
    {
        public int? TheatreId { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int UserId { get; set; }
    }
}
