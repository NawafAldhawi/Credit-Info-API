using System.Threading.Tasks;
using Credit_Info_API.Models;
using Credit_Info_API.DTOs;

namespace Credit_Info_API.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterRequest request);
        Task<User> GetUserByEmailAsync(string email);
        bool VerifyPassword(string inputPassword, string hashedPassword);
    }
}
