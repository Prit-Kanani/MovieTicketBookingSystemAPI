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
                .Where(b => b.IsActive == true)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
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

        #region GET BOOKING BY ID
        [HttpGet("{id}")]
        public IActionResult GetBookingById(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.User)
                .Where(b => b.IsActive == true)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
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

        #region GET BOOKED SEATS FOR A SHOW
        [HttpGet("show/{showId}/seats")]
        public IActionResult GetBookedSeats(int showId)
        {
            var seats = _context.SeatsBookeds
                .Where(s => s.Booking.ShowId == showId && s.Booking.IsActive == true)
                .Select(s => s.SeatNo)
                .ToList();

            return Ok(seats);
        }
        #endregion

        #region CANCEL BOOKING (Soft Delete)
        [HttpDelete("{id}")]
        public IActionResult CancelBooking(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .FirstOrDefault(b => b.BookingId == id && b.IsActive);

            if (booking == null)
                return NotFound(new { message = "Booking not found or already cancelled." });

            booking.IsActive = false;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = inner });
            }
        }
        #endregion

        #region GET BOOKINGS BY USER
        [HttpPost("booking")]
        public async Task<ActionResult<BookingResultDTO>> CreateBooking([FromBody] CreateBookingDTO dto) // <-- add [FromBody]
        {
            try
            {
                if (dto.SeatNos == null || dto.SeatNos.Count == 0)
                    return BadRequest(new BookingResultDTO { Message = "No seats selected." });

                var show = await _context.ShowTimes
                    .Include(s => s.Screen)
                    .FirstOrDefaultAsync(s => s.ShowId == dto.ShowId && s.IsActive);

                if (show == null)
                    return NotFound(new BookingResultDTO { Message = "Show not found or inactive." });

                if (dto.SeatNos.Any(n => n < 1 || n > show.Screen.TotalSeats))
                    return BadRequest(new BookingResultDTO { Message = "One or more seat numbers are invalid." });

                using var tx = await _context.Database.BeginTransactionAsync();

                var alreadyBooked = await _context.Bookings
                    .Where(b => b.ShowId == dto.ShowId && b.IsActive)
                    .SelectMany(b => b.SeatsBookeds.Select(sb => sb.SeatNo))
                    .ToListAsync();

                var conflicts = dto.SeatNos.Intersect(alreadyBooked).Distinct().ToList();
                if (conflicts.Any())
                {
                    await tx.RollbackAsync();
                    return Conflict(new BookingResultDTO
                    {
                        Message = "Some seats have just been booked by others.",
                        Conflicts = conflicts
                    });
                }

                var booking = new Booking
                {
                    UserId = dto.UserId,
                    ShowId = dto.ShowId,
                    DateTime = DateTime.UtcNow,
                    PaymentStatus = dto.PaymentStatus,
                    IsActive = true,
                    SeatsBookeds = dto.SeatNos.Distinct().Select(seat => new SeatsBooked
                    {
                        SeatNo = seat
                    }).ToList()
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new BookingResultDTO
                {
                    BookingId = booking.BookingId,
                    ConfirmedSeats = dto.SeatNos.Distinct().ToList(),
                    Message = "Booking confirmed."
                });
            }
            catch (Exception ex)
            {
                // log it (or return details in dev mode)
                return StatusCode(500, new BookingResultDTO { Message = $"Internal error: {ex.Message}" });
            }
        }


        #endregion

        #region GET BOOKING BY ID FOR TICKET PDF
        [HttpGet("{id}/Pdf")]
        public IActionResult GetBookingByIdPdf(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.SeatsBookeds)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.User)
                .Where(b => b.IsActive == true)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.BookingId,
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
    }
}
