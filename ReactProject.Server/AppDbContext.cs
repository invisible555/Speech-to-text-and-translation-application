using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Entities;
using ReactProject.Server.Model;

namespace ReactProject.Server
{
    public class AppDbContext:DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
        public DbSet<UserAccessTokens> UserAccessTokens { get; set; }

    }
}
