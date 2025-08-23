using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        
        #region GET ALL SHOWTIMES
        [HttpGet]
            public IActionResult GetShowTimes()
            {
            var shows = _context.ShowTimes
                        .Where(s => s.IsActive == true)
                        .ToList();

            return Ok(shows);
            }
        #endregion

        #region GET SHOWTIME BY ID
        [HttpGet("{id}")]
            public IActionResult GetShowTimeByID(int id)
            {
                var show = _context.ShowTimes.Find(id);
                if (show == null || show.IsActive == false)
                    return NotFound();

                return Ok(show);
            }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT SHOWTIME
        [HttpPost]
        public IActionResult InsertShowTime([FromBody] ShowTime show)
        {
            try
            {
                _context.ShowTimes.Add(show);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetShowTimeByID), new { id = show.ShowId }, show);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding showtime.", error = ex.Message });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE SHOWTIME
        [HttpPut("{id}")]
        public IActionResult UpdateShowTime(int id, [FromBody] ShowTime show)
        {
            if (id != show.ShowId)
                return BadRequest("ShowTime ID mismatch.");

            var existing = _context.ShowTimes.Find(id);
            if (existing == null || existing.IsActive == false)
                return NotFound();

            existing.MovieId = show.MovieId;
            existing.ScreenId = show.ScreenId;
            existing.Date = show.Date;
            existing.Time = show.Time;
            existing.Price = show.Price;

            _context.SaveChanges();
            return NoContent();
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

    }
}
