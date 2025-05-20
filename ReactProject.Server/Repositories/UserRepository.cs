using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Model;

namespace ReactProject.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Login == login);
            
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _dbContext.User.AddAsync(user);
        }

       

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.User.FirstOrDefaultAsync(u => u.Id == id);
        }
       
    }
}
