using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Credit_Info_API.Data;
using Credit_Info_API.Models;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt;

namespace Credit_Info_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists" });

            // Hash the password (install BCrypt.Net-Next)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create a new User entity
            var newUser = new User
            {
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = model.Role
            };

            // Save to DB
            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // POST: api/auth/register
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser == null)
                return BadRequest(new { message = "User Does Not Exist" });

            //then verify
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, existingUser.PasswordHash);
            if (!isPasswordCorrect)
                return Unauthorized(new { message = "Invalid password" });

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    existingUser.Id,
                    existingUser.Email,
                    existingUser.Role
                }
            }
            );

            // later will give JWT here
        }

    }
}
