using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Credit_Info_API.Data;
using Credit_Info_API.Models;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Credit_Info_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            // Hash the password 
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

            var token = GenerateJwtToken(existingUser);

            return Ok(new
            {
                message = "Login successful",
                token = token,
                user = new
                {
                    existingUser.Id,
                    existingUser.Email,
                    existingUser.Role
                }
            });


        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
