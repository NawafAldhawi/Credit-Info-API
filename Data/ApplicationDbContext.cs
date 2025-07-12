using Microsoft.EntityFrameworkCore;
using Credit_Info_API.Models;


namespace Credit_Info_API.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> User { get; set; }
        public DbSet<FreezeRequest> FreezRequest { get; set; }
        public DbSet<CreditScore> CreditScore { get; set; }
    }
}
