using ReactProject.Server.DTO;
using ReactProject.Server.Model;
using System.Security.Claims;

namespace ReactProject.Server.Services
{
    public interface IUserService
    {
        Task<AuthenticationResult?> Authenticate(string login, string password);
        Task<RegisterResult> RegisterUser(RegisterRequest model);
        Task<bool> LogoutAsync(int userId);
        string? GetUserRole(ClaimsPrincipal user);
        string? GetUserId(ClaimsPrincipal user);
        
        Task SaveUserTokens(int userId, string accessToken, string refreshToken);
    }
}
