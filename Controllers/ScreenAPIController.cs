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
    public class ScreenAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public ScreenAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        [Authorize]
        #region GET ALL SCREENS
        [HttpGet]
        public IActionResult GetScreens()
        {
            var screens = _context.Screens.ToList();
            return Ok(screens);
        }
        #endregion

        [Authorize]
        #region GET SCREEN BY ID
        [HttpGet("{id}")]
        public IActionResult GetScreenByID(int id)
        {
            var screen = _context.Screens.Find(id);
            if (screen == null)
            {
                return NotFound();
            }
            return Ok(screen);
        }
        #endregion

        [Authorize]
        #region GET SCREENS BY THEATRE ID
        [HttpGet("theatre/{theatreId}")]
        public IActionResult GetScreensByTheatre(int theatreId)
        {
            var screens = _context.Screens
                .Where(s => s.TheatreId == theatreId)
                .Select(s => new ScreenDTO
                {
                    ScreenId = s.ScreenId,
                    TheatreId = s.TheatreId,
                    ScreenNo = s.ScreenNo,
                    TotalSeats = s.TotalSeats,
                    ShowTimes = s.ShowTimes.Count(),  // only count
                    Theatre = s.Theatre               // optional, can be null or minimal info
                })
                .ToList();

            return Ok(screens);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT SCREEN
        [HttpPost]
        public IActionResult InsertScreen([FromBody] Screen screen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Screens.Add(screen);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetScreenByID), new { id = screen.ScreenId }, screen);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE SCREEN
        [HttpPut("{id}")]
        public IActionResult UpdateScreen(int id, [FromBody] Screen screen)
        {
            if (id != screen.ScreenId)
            {
                return BadRequest("Screen ID mismatch.");
            }

            var existingScreen = _context.Screens.Find(id);
            if (existingScreen == null)
            {
                return NotFound();
            }

            existingScreen.ScreenNo = screen.ScreenNo;
            existingScreen.TotalSeats = screen.TotalSeats;
            existingScreen.TheatreId = screen.TheatreId;

            _context.SaveChanges();
            return NoContent();
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE SCREEN
        [HttpDelete("{id}")]
        public IActionResult DeleteScreen(int id)
        {
            var screen = _context.Screens.Find(id);
            if (screen == null)
            {
                return NotFound();
            }

            _context.Screens.Remove(screen);
            _context.SaveChanges();
            return NoContent();
        }
        #endregion


    }
}
