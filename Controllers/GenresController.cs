using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

namespace Movie_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public GenresController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        
        #region GET ALL GENRES
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres);
        }
        #endregion

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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null)
                return NotFound(new { message = "Genre not found." });

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Genre deleted successfully." });
        }
        #endregion

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

        #region PAGINATION OF GENRES
        [HttpGet("paginate")]
        public async Task<IActionResult> PaginateGenres([FromQuery] int pageNumber = 1)
        {
            int pageSize = 10;
            var totalGenres = await _context.Genres.CountAsync();
            var totalPages = (int)Math.Ceiling(totalGenres / (double)pageSize);

            var genres = await _context.Genres
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = genres,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            });
        }
        #endregion

    }
}
