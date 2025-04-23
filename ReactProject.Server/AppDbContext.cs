using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Model;

namespace ReactProject.Server
{
    public class AppDbContext:DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; } = null!;

    }
}
