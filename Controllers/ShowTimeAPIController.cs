using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

namespace Movie_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShowTimeAPIController : ControllerBase
    {
        #region CONFIGURATION
            private readonly MovieManagementContext _context;
            public ShowTimeAPIController(MovieManagementContext context)
            {
                _context = context;
            }
        #endregion

        #region GET SHOW BY SHOW ID
        [HttpGet("Show/{id}")]
        public IActionResult GetShowTimesByShowID(int id)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var show = _context.ShowTimes
                .Where(s => s.ShowId == id && s.IsActive && s.Date >= today)
                .Select(s => new ShowtimeAddDTO
                {
                    ShowId = s.ShowId,
                    MovieId = s.MovieId,
                    ScreenId = s.ScreenId,
                    Date = s.Date,
                    Time = s.Time,
                    Price = s.Price
                })
                .FirstOrDefault();

            if (show == null)
                return NotFound(new APIResponce { Message = "Showtime not found" });

            return Ok(show);
        }
        #endregion


        #region GET SHOWTIME BY SCREEN ID
        [HttpGet("{id}")]
        public IActionResult GetShowTimesByScreenID(int id)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var shows = _context.ShowTimes
                .Where(s => s.ScreenId == id
                            && s.IsActive == true
                            && s.Date >= today
                            )   // ✅ only today & future shows
                .Select(s => new ShowtimeDTO
                {
                    ShowId = s.ShowId,
                    MovieId = s.MovieId,
                    ScreenId = s.ScreenId,
                    Date = s.Date,
                    Time = s.Time,
                    Price = s.Price,
                    IsActive = s.IsActive,
                    MovieName = s.Movie.Name,
                    BookingsCount = s.Bookings.Count(),
                })
                .ToList();

            if (shows == null || !shows.Any())
                return NotFound();

            return Ok(shows);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE SHOWTIME
        [HttpPut]
        public async Task<IActionResult> UpdateShowtime(ShowtimeAddDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate date & time
            if (dto.Date < DateOnly.FromDateTime(DateTime.Today) ||
                (dto.Date == DateOnly.FromDateTime(DateTime.Today) && dto.Time <= TimeOnly.FromDateTime(DateTime.Now)))
            {
                return BadRequest(new { message = "Showtime must be in the future." });
            }

            // Validate price
            if (dto.Price < 0)
                return BadRequest(new { message = "Price cannot be negative." });

            var existingShowtime = await _context.ShowTimes.FirstOrDefaultAsync(s => s.ShowId == dto.ShowId);
            if (existingShowtime == null || !existingShowtime.IsActive)
                return NotFound(new { message = $"Showtime with ID {dto.ShowId} not found or inactive." });

            // Validate Movie
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId && m.IsActive);
            if (!movieExists)
                return NotFound(new { message = $"Movie with ID {dto.MovieId} not found or inactive." });

            // Validate Screen
            var screenExists = await _context.Screens.AnyAsync(s => s.ScreenId == dto.ScreenId && s.IsActive);
            if (!screenExists)
                return NotFound(new { message = $"Screen with ID {dto.ScreenId} not found or inactive." });

            // Prevent duplicate conflict (same screen/date/time, different ShowId)
            var conflict = await _context.ShowTimes.AnyAsync(s =>
                s.ScreenId == dto.ScreenId &&
                s.Date == dto.Date &&
                s.Time == dto.Time &&
                s.ShowId != dto.ShowId &&
                s.IsActive);

            if (conflict)
                return BadRequest(new { message = "Another showtime already exists for this screen at the same date and time." });

            // Update fields
            existingShowtime.MovieId = dto.MovieId;
            existingShowtime.ScreenId = dto.ScreenId;
            existingShowtime.Date = dto.Date;
            existingShowtime.Time = dto.Time;
            existingShowtime.Price = dto.Price;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Showtime updated successfully!" });
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE SHOWTIME
        [HttpDelete("{id}")]
        public IActionResult DeleteShowTime(int id)
        {
            var show = _context.ShowTimes.Find(id);
            if (show == null || show.IsActive == false)
                return NotFound();
            show.IsActive = false;
            _context.SaveChanges();
            return NoContent();
        }
        #endregion

        [AllowAnonymous]
        #region FILTER SHOWTIMES
        [HttpGet("filter")]
        public IActionResult FilterShowTimes(
            [FromQuery] int? movieId,
            [FromQuery] int? theatreId,
            [FromQuery] DateTime? date)
        {
            var query = _context.ShowTimes
                    .Include(s => s.Screen)
                        .ThenInclude(screen => screen.Theatre)
                    .Where(s => s.IsActive == true)
                    .AsQueryable();

            if (movieId.HasValue)
                query = query.Where(s => s.MovieId == movieId.Value);

            if (theatreId.HasValue)
                query = query.Where(s => s.Screen.TheatreId == theatreId.Value);

            if (date.HasValue)
            {
                var dateOnly = DateOnly.FromDateTime(date.Value);
                query = query.Where(s => s.Date == dateOnly);
            }

            var results = query.ToList();
            return Ok(results);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT SHOWTIME
        [HttpPost]
        public async Task<IActionResult> InsertShowtime([FromBody] ShowtimeAddDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate date & time
            if (dto.Date < DateOnly.FromDateTime(DateTime.Today) ||
                (dto.Date == DateOnly.FromDateTime(DateTime.Today) && dto.Time <= TimeOnly.FromDateTime(DateTime.Now)))
            {
                return BadRequest(new { message = "Showtime must be in the future." });
            }

            // Validate price
            if (dto.Price < 0)
                return BadRequest(new { message = "Price cannot be negative." });

            // Validate Movie
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId && m.IsActive);
            if (!movieExists)
                return NotFound(new { message = $"Movie with ID {dto.MovieId} not found or inactive." });

            // Validate Screen
            var screenExists = await _context.Screens.AnyAsync(s => s.ScreenId == dto.ScreenId && s.IsActive);
            if (!screenExists)
                return NotFound(new { message = $"Screen with ID {dto.ScreenId} not found or inactive." });

            // Check for existing showtime (same screen, same date & time)
            var existingShowtime = await _context.ShowTimes
                .FirstOrDefaultAsync(s => s.ScreenId == dto.ScreenId
                                       && s.Date == dto.Date
                                       && s.Time == dto.Time);

            if (existingShowtime != null)
            {
                if (existingShowtime.IsActive)
                    return BadRequest(new { message = "A showtime already exists for this screen at the same date and time." });

                // Reactivate instead of creating duplicate
                existingShowtime.IsActive = true;
                existingShowtime.MovieId = dto.MovieId;
                existingShowtime.Price = dto.Price;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Showtime reactivated successfully!" });
            }

            // Create new showtime
            var newShowtime = new ShowTime
            {
                MovieId = dto.MovieId,
                ScreenId = dto.ScreenId,
                Date = dto.Date,
                Time = dto.Time,
                Price = dto.Price,
                IsActive = true
            };

            await _context.ShowTimes.AddAsync(newShowtime);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Showtime added successfully!" });
        }
        #endregion


    }
}
