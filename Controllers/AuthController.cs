using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Credit_Info_API.Data;
using Credit_Info_API.Models;
using Credit_Info_API.DTOs;
using Credit_Info_API.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Credit_Info_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IJwtService _JwtService;
        private readonly IAdminServices _adminService;


        public AuthController(ApplicationDbContext context, IConfiguration configuration, IUserService userService, 
            IJwtService jwtService, IAdminServices adminService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _JwtService = jwtService;
            _adminService = adminService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists" });

            var user = await _userService.RegisterUserAsync(model);
            return Ok(new { message = "User registered successfully", user.Id, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userService.GetUserByEmailAsync(model.Email);
            if (existingUser == null)
                return BadRequest(new { message = "User Does Not Exist" });

            var isPasswordCorrect = _userService.VerifyPassword(model.Password, existingUser.PasswordHash);
            if (!isPasswordCorrect)
                return Unauthorized(new { message = "Invalid password" });

            var token = _JwtService.GenerateJwtToken(existingUser);

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

    }
}
