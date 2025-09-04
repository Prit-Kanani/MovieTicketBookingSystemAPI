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

        // New fields for ticket generation:
        public DateOnly? ShowDate { get; set; }            // or string if your model uses string
        public TimeOnly? ShowTime { get; set; }            // or string
        public decimal Price { get; set; }                 // price per seat
        public string TheatreName { get; set; } = "";
        public int ScreenNo { get; set; }
    }
    public class BookingResultDTO
    {
        public int BookingId { get; set; }
        public List<int> ConfirmedSeats { get; set; } = new();
        public List<int> Conflicts { get; set; } = new();
        public string Message { get; set; } = "";
    }
    public class CreateBookingDTO
    {
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public List<int> SeatNos { get; set; } = new();
        public string PaymentStatus { get; set; } = "Pending"; // or "Paid" from your UI flow
    }
}
