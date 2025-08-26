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
    public class TheatreAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public TheatreAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL THEATRES
        [HttpGet]
        public IActionResult GetTheaters()
        {
            var theatres = _context.Theatres
                .Include(t => t.Screens)
                .Where(t => t.IsActive == true)
                .Select(t => new
                {
                    t.TheatreId,
                    t.Name,
                    t.City,
                    t.Address,
                    Screens = t.Screens.Where(s => s.IsActive == true).Select(s => new
                    {
                        s.ScreenNo
                    }).ToList()
                })
                .ToList();
            return Ok(theatres);
        }
        #endregion

        #region GET THEATRE BY ID
        [HttpGet("{id}")]
        public IActionResult GetTheatreByID(int id)
        {
            var theatre = _context.Theatres.Find(id);
            if (theatre == null || theatre.IsActive == false)
            {
                return NotFound();
            }
            return Ok(theatre);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE THEATRE BY IDS (Soft Delete)
        [HttpPost("DeleteTheatres")]
        public IActionResult DeleteTheatres([FromBody] List<int> theatreIds)
        {
            if (theatreIds == null || !theatreIds.Any())
                return BadRequest(new { message = "No IDs provided." });

            var theatres = _context.Theatres
                .Where(t => theatreIds.Contains(t.TheatreId) && t.IsActive == true)
                .ToList();

            if (!theatres.Any())
                return BadRequest(new { message = "No theatres found to delete." });

            int requestedCount = theatreIds.Count;
            int foundCount = theatres.Count;

            // 🔥 Soft delete instead of RemoveRange
            foreach (var theatre in theatres)
            {
                theatre.IsActive = false;
            }

            _context.SaveChanges();

            if (requestedCount == foundCount)
            {
                return Ok(new { message = "All theatres deactivated successfully." });
            }
            else if (foundCount > 0 && foundCount < requestedCount)
            {
                return StatusCode(StatusCodes.Status206PartialContent, new
                {
                    message = "Some theatres deactivated, some not found."
                });
            }
            else
            {
                return BadRequest(new { message = "No theatres were deactivated." });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT THEATRE
        [HttpPost]
        public async Task<IActionResult> InsertTheatre([FromBody] TheatreDTO theatre)
        {
            // Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Normalize name to avoid case-sensitive duplicates
            var normalizedName = theatre.Name.Trim().ToLower();

            // Check if an active theatre with the same name already exists
            var existingTheatre = await _context.Theatres
                .FirstOrDefaultAsync(t => t.IsActive == true && t.Name.ToLower() == normalizedName);

            if (existingTheatre != null)
                return BadRequest(new { message = "Theatre already exists." });

            try
            {
                var newTheatre = new Theatre
                {
                    Name = theatre.Name.Trim(),
                    City = theatre.City.Trim(),
                    Address = theatre.Address.Trim(),
                    UserId = theatre.UserId,
                    IsActive = true
                };

                await _context.Theatres.AddAsync(newTheatre);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Theatre added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving theatre.", error = ex.Message });
            }
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE THEATRE
        [HttpPut("{id}")]
        public IActionResult UpdateTheatre(int id, TheatreDTO theatre)
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
                .Where(t => (string.IsNullOrEmpty(city) || t.City.ToLower().Contains(city.ToLower())) && t.IsActive == true)
                .ToList();

            return Ok(theatres);
        }

        #endregion

        #region GET THEATERS BY USER
        [HttpGet("user/{userId}")]
        public IActionResult GetTheatresManagedByUser(int userId)
        {
            var theatres = _context.Theatres.Where(t => t.UserId == userId && t.IsActive == true).ToList();
            return Ok(theatres);
        }
        #endregion
    }
}
