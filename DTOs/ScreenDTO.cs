using Movie_Management_API.Models;

namespace Movie_Management_API.DTOs
{
    public class ScreenDTO
    {
        public int ScreenId { get; set; }

        public int TheatreId { get; set; }

        public int ScreenNo { get; set; }

        public int TotalSeats { get; set; }

        public virtual int? ShowTimes { get; set; }

        public virtual Theatre Theatre { get; set; } = null!;
    }
    public class ScreenAddDTO
    {
        public int? ScreenId { get; set; }
        public int TheatreId { get; set; }
        public int ScreenNo { get; set; }
        public int TotalSeats { get; set; }
    }
}
