using ReactProject.Server.Model;

namespace ReactProject.Server.Repositories
{
    public interface IUserRepository
    {
        Task<Users?> GetByLoginAsync(string login);
        Task<Users?> GetByEmailAsync(string email);
        Task AddUserAsync(Users user);
        Task SaveChangesAsync();
    }
}
