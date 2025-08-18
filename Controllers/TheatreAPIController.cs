using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

namespace Movie_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatreAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public TheatreAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        [Authorize]
        #region GET ALL THEATRES
        [HttpGet]
        public IActionResult GetTheaters()
        {
            var theatres = _context.Theatres
                .Include(t => t.Screens)
                .Select(t => new
                {
                    t.TheatreId,
                    t.Name,
                    t.City,
                    t.Address,
                    Screens = t.Screens.Select(s => new
                    {
                        s.ScreenNo
                    }).ToList()
                })
                .ToList();
            return Ok(theatres);
        }
        #endregion

        [Authorize]
        #region GET THEATRE BY ID
        [HttpGet("{id}")]
        public IActionResult GetTheatreByID(int id)
        {
            var theatre = _context.Theatres.Find(id);
            if (theatre == null)
            {
                return NotFound();
            }
            return Ok(theatre);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE THEATRE BY IDS
        [HttpPost("DeleteTheatres")]
        public IActionResult DeleteTheatres([FromBody] List<int> theatreIds)
        {
            if (theatreIds == null || !theatreIds.Any())
                return BadRequest(new { message = "No IDs provided." });

            var theatres = _context.Theatres.Where(t => theatreIds.Contains(t.TheatreId)).ToList();

            if (!theatres.Any())
                return BadRequest(new { message = "No theatres found to delete." });

            int requestedCount = theatreIds.Count;
            int foundCount = theatres.Count;

            _context.Theatres.RemoveRange(theatres);
            _context.SaveChanges();

            if (requestedCount == foundCount)
            {
                return Ok(new { message = "All theatres deleted successfully." });
            }
            else if (foundCount > 0 && foundCount < requestedCount)
            {
                return StatusCode(StatusCodes.Status206PartialContent, new
                {
                    message = "Some theatres deleted, some not found."
                });
            }
            else
            {
                return BadRequest(new { message = "No theatres were deleted." });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT THEATRE
        [HttpPost]
        public async Task<IActionResult> InsertTheatre(TheatreDTO theatre)
        {
            var existingtheatre = await _context.Theatres.FirstOrDefaultAsync(t => t.Name == theatre.Name);
            if (existingtheatre != null)
                return BadRequest(new { message = "Theatre already exists." });
            try
            {
                var newTheatre = new Theatre
                {
                    Name = theatre.Name,
                    City = theatre.City,
                    Address = theatre.Address,
                    UserId = theatre.UserId
                };
                _context.Theatres.Add(newTheatre);
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Error saving theatre.");
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE THEATRE
        [HttpPut("{id}")]
        public IActionResult UpdateTheatre(int id, Theatre theatre)
        {
            var existingTheatre= _context.Theatres.Find(id);
            if (existingTheatre == null)
            {
                return NotFound();
            }
            existingTheatre.Name        = theatre.Name;
            existingTheatre.City        = theatre.City;
            existingTheatre.Address     = theatre.Address;
            existingTheatre.UserId      = theatre.UserId;

            _context.Theatres.Update(existingTheatre);
            _context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region FILTER THEATRE BY CITY
        [HttpGet("filter")]
        public IActionResult FilterTheatres([FromQuery] string city)
        {
            var theatres = _context.Theatres
                .Where(t => string.IsNullOrEmpty(city) || t.City.ToLower().Contains(city.ToLower()))
                .ToList();

            return Ok(theatres);
        }

        #endregion

        #region GET THEATERS BY USER
        [HttpGet("user/{userId}")]
        public IActionResult GetTheatresManagedByUser(int userId)
        {
            var theatres = _context.Theatres.Where(t => t.UserId == userId).ToList();
            return Ok(theatres);
        }
        #endregion
    }
}
