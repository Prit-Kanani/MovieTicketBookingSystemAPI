namespace Movie_Management_API.DTOs
{
    public class MovieDetailsResponseDTO
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public List<string> Genres { get; set; }
        public List<MovieTheatreResponseDTO> Theatres { get; set; }
    }

    public class MovieTheatreResponseDTO
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public List<MovieScreenResponseDTO> Screens { get; set; }
    }

    public class MovieScreenResponseDTO
    {
        public int ScreenId { get; set; }
        public int ScreenNo { get; set; }
        public int TotalSeats { get; set; }
        public List<MovieShowTimeResponseDTO> ShowTimes { get; set; }
    }

    public class MovieShowTimeResponseDTO
    {
        public int ShowId { get; set; }
        public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
        public decimal Price { get; set; }
    }
}
