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
        public List<string> SeatNos { get; set; } = new();
    }
}
