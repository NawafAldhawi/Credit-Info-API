using Credit_Info_API.Models;

namespace Credit_Info_API.Services
{
    public interface IJwtService
    {

        string GenerateJwtToken(User user);
    }
}
