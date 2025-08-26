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
    public class ScreenAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public ScreenAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion
       
        #region GET ALL SCREENS
        [HttpGet]
        public IActionResult GetScreens()
        {
            var screens = _context.Screens.Where(s => s.IsActive == true).ToList();
            return Ok(screens);
        }
        #endregion

        #region GET SCREEN BY ID
        [HttpGet("{id}")]
        public IActionResult GetScreenByID(int id)
        {
            var screen = _context.Screens.Find(id);
            if (screen == null || screen.IsActive == false)
            {
                return NotFound();
            }
            return Ok(screen);
        }
        #endregion

        #region GET SCREENS BY THEATRE ID
        [HttpGet("theatre/{theatreId}")]
        public IActionResult GetScreensByTheatre(int theatreId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            var screens = _context.Screens
                .Where(s => s.TheatreId == theatreId && s.IsActive == true)
                .Select(s => new ScreenDTO
                {
                    ScreenId = s.ScreenId,
                    TheatreId = s.TheatreId,
                    ScreenNo = s.ScreenNo,
                    TotalSeats = s.TotalSeats,
                    ShowTimes = s.ShowTimes
                        .Where(st => st.IsActive == true
                                     && (st.Date > today
                                         || (st.Date == today && st.Time > currentTime))) // ✅ only future shows
                        .Count(),
                    Theatre = s.Theatre
                })
                .ToList();

            return Ok(screens);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT SCREEN
        [HttpPost]
        public async Task<IActionResult> InsertScreen([FromBody] ScreenAddDTO screen)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if a screen with same ScreenNo exists in this theatre
            var existingScreen = await _context.Screens
                .FirstOrDefaultAsync(s => s.TheatreId == screen.TheatreId
                                       && s.ScreenNo == screen.ScreenNo);

            if (existingScreen != null)
            {
                if (existingScreen.IsActive)
                {
                    return BadRequest(new { message = "Screen already exists in this theatre." });
                }

                // Reactivate and update fields
                existingScreen.IsActive = true;
                existingScreen.TotalSeats = screen.TotalSeats;

                // if later you add more props like ProjectionType, SoundSystem etc.
                // update them here as well.

                await _context.SaveChangesAsync();
                return Ok(new { message = "Screen reactivated and updated successfully!" });
            }

            // If no existing screen, create new
            var newScreen = new Screen
            {
                TheatreId = screen.TheatreId,
                ScreenNo = screen.ScreenNo,
                TotalSeats = screen.TotalSeats,
                IsActive = true
            };

            await _context.Screens.AddAsync(newScreen);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Screen added successfully!" });
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE SCREEN
        [HttpPut]
        public async Task<IActionResult> UpdateScreen(ScreenAddDTO screen)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingScreen = await _context.Screens.FindAsync(screen.ScreenId);

            if (existingScreen == null)
                return NotFound(new { message = "Screen not found." });

            // Check duplicate ScreenNo in the same Theatre
            var duplicate = await _context.Screens
                .FirstOrDefaultAsync(s => s.TheatreId == screen.TheatreId
                                       && s.ScreenNo == screen.ScreenNo
                                       && s.ScreenId != screen.ScreenId);

            if (duplicate != null)
                return BadRequest(new { message = "A screen with this number already exists in this theatre." });

            existingScreen.ScreenNo = screen.ScreenNo;
            existingScreen.TotalSeats = screen.TotalSeats;
            existingScreen.TheatreId = screen.TheatreId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Screen updated successfully!" });
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region SOFT DELETE MULTIPLE SCREENS
        [HttpPost("DeleteScreens")]
        public IActionResult DeleteScreens([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No screen IDs provided.");

            var screens = _context.Screens
                .Where(s => ids.Contains(s.ScreenId) && s.IsActive == true)
                .ToList();

            if (!screens.Any())
                return NotFound("No active screens found for given IDs.");

            foreach (var screen in screens)
            {
                screen.IsActive = false;
            }

            _context.SaveChanges();

            // If all deleted
            if (screens.Count == ids.Count)
                return Ok("All selected screens marked as inactive.");

            // Some found, some missing
            return StatusCode(StatusCodes.Status206PartialContent,
                $"{screens.Count} screens marked as inactive, some IDs not found.");
        }
        #endregion



    }
}
