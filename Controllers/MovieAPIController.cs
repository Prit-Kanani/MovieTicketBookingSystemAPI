using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.Models;
using Movie_Management_API.DTOs; // Make sure this namespace matches your project

namespace Movie_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovieAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;

        public MovieAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL MOVIES (With Genres)
        [HttpGet]
        public IActionResult GetMovies()
        {
            var movies = _context.Movies
                .Include(m => m.Genres)
                .Where(m => m.IsActive == true)
                .Select(m => new MovieDTO
                {
                    MovieId = m.MovieId,
                    Name = m.Name,
                    Language = m.Language,
                    Duration = m.Duration,
                    Poster = m.Poster,
                    Description = m.Description,
                    Genres = m.Genres.Select(g => g.Name).ToList()
                })
                .ToList();

            return Ok(movies);
        }
        #endregion

        #region GET MOVIE BY ID (With Genres)
        [HttpGet("{id}")]
        public IActionResult GetMovieByID(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Genres)
                .Where(m => m.MovieId == id && m.IsActive == true)
                .Select(m => new AddMovieDTO
                {
                    MovieId = m.MovieId,
                    Name = m.Name,
                    Language = m.Language,
                    Duration = m.Duration,
                    Poster = m.Poster,
                    Description = m.Description,
                    GenreIds = m.Genres.Select(g => g.GenreId).ToList()
                })
                .FirstOrDefault();

            if (movie == null)
                return NotFound();

            return Ok(movie);
        }
        #endregion

        #region DELETE MOVIE BY ID (Soft Delete)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteMovieByID(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id && m.IsActive);

            if (movie == null)
                return NotFound(new { message = "Movie not found or already inactive." });

            movie.IsActive = false;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Movie deactivated successfully." });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = inner });
            }
        }
        #endregion

        #region INSERT MOVIE
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] AddMovieDTO dto)
        {
            // Validate GenreIds are not null or empty
            if (dto.GenreIds == null || !dto.GenreIds.Any())
            {
                return BadRequest("At least one genre must be selected.");
            }

            var genres = await _context.Genres
                .Where(g => dto.GenreIds.Contains(g.GenreId))
                .ToListAsync();

            // Check if any invalid GenreIds were sent
            var missingGenreIds = dto.GenreIds.Except(genres.Select(g => g.GenreId)).ToList();
            if (missingGenreIds.Any())
            {
                return BadRequest($"Invalid Genre ID(s): {string.Join(", ", missingGenreIds)}");
            }
            // Create Movie object
            var movie = new Movie
            {
                Name = dto.Name,
                Language = dto.Language,
                Duration = dto.Duration,
                Poster = dto.Poster, 
                Description = dto.Description,
                Genres = genres
            };

            // Add and save
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Movie added successfully",
                Movie = movie
            });
        }


        #endregion

        #region UPDATE MOVIE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, AddMovieDTO movie)
        {
            if (id != movie.MovieId)
                return BadRequest();

            var existingMovie = _context.Movies
                .Where(m => m.IsActive == true)
                .Include(m => m.Genres)
                .FirstOrDefault(m => m.MovieId == id);

            if (existingMovie == null)
                return NotFound();

            // Update basic fields
            existingMovie.Name = movie.Name;
            existingMovie.Language = movie.Language;
            existingMovie.Duration = movie.Duration;
            existingMovie.Poster = movie.Poster;
            existingMovie.Description = movie.Description;

            // Update genres
            existingMovie.Genres.Clear();
            var genreIds = movie.GenreIds;
            var genres = _context.Genres.Where(g => genreIds.Contains(g.GenreId)).ToList();
            foreach (var genre in genres)
            {
                existingMovie.Genres.Add(genre);
            }

            _context.SaveChanges();
            return NoContent();
        }

        #endregion

        #region GET MOVIE FULL DETAILS (with Genres, Theatres, Screens, ShowTimes)
        [HttpGet("{id}/fulldetails")]
        public IActionResult GetMovieFullDetails(int id)
        {
            DeactivatePastShows(); // Soft delete past shows

            var movie = _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.ShowTimes)
                    .ThenInclude(st => st.Screen)   
                        .ThenInclude(s => s.Theatre)
                .Where(m => m.MovieId == id && m.IsActive == true)
                .Select(m => new MovieDetailsResponseDTO
                {
                    MovieId = m.MovieId,
                    Name = m.Name,
                    Language = m.Language,
                    Duration = m.Duration,
                    Poster = m.Poster,
                    Description = m.Description,
                    Genres = m.Genres.Select(g => g.Name).ToList(),

                    Theatres = m.ShowTimes
                        .Where(st => st.IsActive == true && st.Screen.IsActive == true && st.Screen.Theatre.IsActive == true)
                        .GroupBy(st => st.Screen.Theatre)
                        .Select(g => new MovieTheatreResponseDTO
                        {
                            TheatreId = g.Key.TheatreId,
                            TheatreName = g.Key.Name,
                            City = g.Key.City,
                            Address = g.Key.Address,
                            Screens = g.GroupBy(st => st.Screen)
                                .Select(sg => new MovieScreenResponseDTO
                                {
                                    ScreenId = sg.Key.ScreenId,
                                    ScreenNo = sg.Key.ScreenNo,
                                    TotalSeats = sg.Key.TotalSeats,
                                    ShowTimes = sg.Select(st => new MovieShowTimeResponseDTO
                                    {
                                        ShowId = st.ShowId,
                                        Date = st.Date,
                                        Time = st.Time,
                                        Price = st.Price
                                    }).ToList()
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefault();

            if (movie == null)
                return NotFound(new { message = "Movie not found or inactive." });

            return Ok(movie);
        }
        #endregion

        #region DELETE PAST SHOWTIMES (Soft Delete)
        private void DeactivatePastShows()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var pastShows = _context.ShowTimes
                .Where(s => s.Date < today && s.IsActive) // only active but expired
                .ToList();

            if (pastShows.Any())
            {
                foreach (var show in pastShows)
                {
                    show.IsActive = false;
                }

                _context.SaveChanges();
            }
        }
        #endregion

    }
}
