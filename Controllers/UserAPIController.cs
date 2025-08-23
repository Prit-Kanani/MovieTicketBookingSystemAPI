using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

namespace Movie_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        #region CONFIGURATION
        private readonly MovieManagementContext _context;
        public UserAPIController(MovieManagementContext context)
        {
            _context = context;
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region GET ALL USERS
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    BookingCount = u.Bookings.Count()
                })
                .ToList();
            return Ok(users);
        }
        #endregion

        [Authorize]
        #region GET USER BY ID
        [HttpGet("{id}")]
        public IActionResult GetUserByID(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region INSERT ADMIN USER
        [HttpPost]
        public async Task<IActionResult> AddUser(UserAddDTO newUser)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists." });
            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Name  = newUser.Name,
                Email = newUser.Email,
                Role  = newUser.Role
            };

            user.Password = hasher.HashPassword(user, newUser.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User added successfully!." });
        }

        #endregion

        [Authorize(Roles = "Admin")]
        #region UPDATE USER
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserEditDTO user)
        {
            var existingUser = _context.Users.Find(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                var hasher = new PasswordHasher<User>();
                existingUser.Password = hasher.HashPassword(existingUser, user.Password);
            }

            _context.Users.Update(existingUser);
            _context.SaveChanges();
            return NoContent();
        }
        #endregion

        [Authorize(Roles = "Admin")]
        #region DELETE USER BY ID
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting Admin accounts
            if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot delete an Admin account.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
        #endregion


    }
}
