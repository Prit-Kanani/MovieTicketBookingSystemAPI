using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

namespace Movie_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingAPIController : ControllerBase
    {
        #region CONFIGURATION
            private readonly MovieManagementContext _context;

            public BookingAPIController(MovieManagementContext context)
            {
                _context = context;
            }
        #endregion

        [Authorize(Roles ="Admin")]
        #region GET ALL BOOKINGS
        [HttpGet]
        public IActionResult GetAllBookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.User)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
                    BookingType = b.BookingType,
                    PaymentStatus = b.PaymentStatus,
                    DateTime = (DateTime)b.DateTime,
                    SeatNos = b.SeatsBookeds.Select(s => s.SeatNo).ToList(),
                    MovieName = b.Show.Movie.Name,
                    UserName = b.User.Name
                })
                .ToList();


            return Ok(bookings);
        }
        #endregion

        [Authorize]
        #region GET BOOKING BY ID
        [HttpGet("{id}")]
        public IActionResult GetBookingById(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.User)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
                    BookingType = b.BookingType,
                    PaymentStatus = b.PaymentStatus,
                    DateTime = (DateTime)b.DateTime,
                    SeatNos = b.SeatsBookeds.Select(s => s.SeatNo).ToList(),
                    MovieName = b.Show.Movie.Name,
                    UserName = b.User.Name
                })
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }
        #endregion

        [Authorize]
        #region GET BOOKED SEATS FOR A SHOW
        [HttpGet("show/{showId}/seats")]
        public IActionResult GetBookedSeats(int showId)
        {
            var seats = _context.SeatsBookeds
                .Where(s => s.Booking.ShowId == showId)
                .Select(s => s.SeatNo)
                .ToList();

            return Ok(seats);
        }
        #endregion

        [Authorize]
        #region CREATE BOOKING
        [HttpPost]
        public IActionResult CreateBooking([FromBody] BookingDTO dto)
        {
            try
            {
                // Check for seat conflicts
                var alreadyBooked = _context.SeatsBookeds
                    .Where(s => s.Booking.ShowId == dto.ShowId)
                    .Select(s => s.SeatNo)
                    .ToList();

                var duplicateSeats = dto.SeatNos.Intersect(alreadyBooked).ToList();
                if (duplicateSeats.Any())
                {
                    return Conflict(new
                    {
                        message = "Some seats are already booked.",
                        seats = duplicateSeats
                    });
                }

                // Create booking
                var booking = new Booking
                {
                    UserId = dto.UserId,
                    ShowId = dto.ShowId,
                    BookingType = dto.BookingType,
                    PaymentStatus = dto.PaymentStatus,
                    DateTime = DateTime.Now,
                    SeatsBookeds = dto.SeatNos.Select(seat => new SeatsBooked
                    {
                        SeatNo = seat
                    }).ToList()
                };

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Booking failed.",
                    error = ex.Message
                });
            }
        }
        #endregion
        [Authorize]
        #region DELETE BOOKING
        [HttpDelete("{id}")]
        public IActionResult CancelBooking(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            _context.SeatsBookeds.RemoveRange(booking.SeatsBookeds);
            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return NoContent();
        }
        #endregion
    }
}
