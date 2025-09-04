using Movie_Management_API.Models;

namespace Movie_Management_API.DTOs
{
    public class ShowtimeDTO
    {
        public int ShowId { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int BookingsCount { get; set; }

        public string  MovieName { get; set; } = null!;
    }
    //----------------------------------------------------------------
    public class ShowSeatMapDTO
    {
        public int ShowId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public string Theatre { get; set; } = "";
        public int ScreenNo { get; set; }

        public List<int> MyBookedSeats { get; set; } = new();
        public List<int> OthersBookedSeats { get; set; } = new();
    }

    public class ShowtimeAddDTO
    {
        public int? ShowId { get; set; }

        public int MovieId { get; set; }

        public int ScreenId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public decimal Price { get; set; }
    }
}
