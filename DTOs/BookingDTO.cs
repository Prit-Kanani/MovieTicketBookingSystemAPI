using System.ComponentModel.DataAnnotations;

namespace Movie_Management_API.DTOs
{
    public class BookingDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ShowId { get; set; }

        [Required]
        public string BookingType { get; set; } = string.Empty;

        [Required]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required]
        public List<int> SeatNos { get; set; } = new();
    }
    public class BookingResponseDTO
    {
        public int BookingId { get; set; }

        [Required]
        public string BookingType { get; set; } = string.Empty;

        [Required]
        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        public List<int> SeatNos { get; set; } = new();

        public string MovieName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
