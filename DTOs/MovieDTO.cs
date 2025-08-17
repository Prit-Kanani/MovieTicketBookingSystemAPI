
using System.ComponentModel.DataAnnotations;
using Movie_Management_API.Models;

namespace Movie_Management_API.DTOs
{
    public class MovieDTO
    {
        public int? MovieId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Language { get; set; }

        public int Duration { get; set; }

        public string Poster { get; set; }

        public string Description { get; set; }

        public virtual List<string> Genres { get; set; }
    }
    public class AddMovieDTO
    {
        public int? MovieId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string Poster { get; set; } 
        public string Description { get; set; }
        public List<int> GenreIds { get; set; } 
    }

}
