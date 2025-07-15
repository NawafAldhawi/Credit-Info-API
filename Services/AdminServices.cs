using Credit_Info_API.Data;
using Credit_Info_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Credit_Info_API.Services
{
    public class AdminServices
    {

        private readonly ApplicationDbContext _context;

        public AdminServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAdminAsync(RegisterRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = "Admin"
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

    }
}
