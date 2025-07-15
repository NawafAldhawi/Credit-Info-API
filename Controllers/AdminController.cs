using Credit_Info_API.Data;
using Credit_Info_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Credit_Info_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAdminServices _AdminService;
        private readonly IJwtService _JwtService;

        public AdminController(ApplicationDbContext context, IConfiguration configuration, IAdminServices AdminService, IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _AdminService = AdminService;
            _JwtService = jwtService;
        }

    }
}
