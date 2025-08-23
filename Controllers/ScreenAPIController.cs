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
            var screens = _context.Screens
                .Where(s => s.TheatreId == theatreId && s.IsActive == true)
                .Select(s => new ScreenDTO
                {
                    ScreenId = s.ScreenId,
                    TheatreId = s.TheatreId,
                    ScreenNo = s.ScreenNo,
                    TotalSeats = s.TotalSeats,
                    ShowTimes = s.ShowTimes.Count(),  
                    Theatre = s.Theatre               
                })
                .ToList();

            return Ok(screens);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT SCREEN
        [HttpPost]
        public IActionResult InsertScreen(ScreenAddDTO screen)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Screen = new Screen
            {
                TheatreId = screen.TheatreId,
                ScreenNo = screen.ScreenNo,
                TotalSeats = screen.TotalSeats
            };

            _context.Screens.Add(Screen);
            _context.SaveChanges();
            return Ok();
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE SCREEN
        [HttpPut("{id}")]
        public IActionResult UpdateScreen([FromBody] Screen screen)
        {

            var existingScreen = _context.Screens.Find(screen.ScreenId);

            if (existingScreen == null || existingScreen.IsActive == false)
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
            if (screen == null || screen.IsActive == false)
            {
                return NotFound();
            }
            screen.IsActive = false;
            _context.SaveChanges();
            return NoContent();
        }
        #endregion


    }
}
