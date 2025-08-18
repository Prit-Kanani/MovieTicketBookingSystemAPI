using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.Models;

namespace Movie_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public GenresController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        [Authorize]
        #region GET ALL GENRES
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres);
        }
        #endregion

        [Authorize]
        #region GET GENRE BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound(new { message = "Genre not found." });

            return Ok(genre);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region CREATE GENRE
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] Genre genre)
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
                return BadRequest(new { message = "Genre name cannot be empty." });

            var existingGenre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == genre.Name.ToLower());

            if (existingGenre != null)
                return BadRequest(new { message = "Genre already exists." });

            try
            {
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Genre created successfully.", genre });
            }
            catch
            {
                return StatusCode(500, new { message = "Error creating genre." });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE GENRE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] Genre genre)
        {
            if (id != genre.GenreId)
                return BadRequest(new { message = "Genre ID mismatch." });

            var existingGenre = await _context.Genres.FindAsync(id);
            if (existingGenre == null)
                return NotFound(new { message = "Genre not found." });

            existingGenre.Name = genre.Name;

            try
            {
                _context.Genres.Update(existingGenre);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Genre updated successfully.", genre = existingGenre });
            }
            catch
            {
                return StatusCode(500, new { message = "Error updating genre." });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE GENRE
        [HttpPost("DeleteGenres")]
        public async Task<IActionResult> DeleteGenres([FromBody] List<int> genreIds)
        {
            if (genreIds == null || !genreIds.Any())
                return BadRequest(new { message = "No IDs provided." });

            var genres = await _context.Genres.Where(g => genreIds.Contains(g.GenreId)).ToListAsync();

            if (!genres.Any())
                return BadRequest(new { message = "No genres found to delete." });

            _context.Genres.RemoveRange(genres);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Selected genres deleted successfully." });
        }
        #endregion

        [Authorize]
        #region FILTER GENRES BY NAME
        [HttpGet("filter")]
        public async Task<IActionResult> FilterGenres([FromQuery] string name)
        {
            var genres = await _context.Genres
                .Where(g => string.IsNullOrEmpty(name) || g.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return Ok(genres);
        }
        #endregion
    }
}
