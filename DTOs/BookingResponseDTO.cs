using System.ComponentModel.DataAnnotations;

namespace Movie_Management_API.DTOs
{
    public class BookingResponseDTO
    {
        public int BookingId { get; set; }

        [Required]
        public string BookingType { get; set; } = string.Empty;

        [Required]
        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }

        public List<string> SeatNos { get; set; } = new();

        public string MovieName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
