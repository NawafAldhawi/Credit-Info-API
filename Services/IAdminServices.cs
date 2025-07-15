using Credit_Info_API.Models;

namespace Credit_Info_API.Services
{
    public interface IAdminServices
    {
          Task<User> RegisterAdminAsync(RegisterRequest request);
    }
}
