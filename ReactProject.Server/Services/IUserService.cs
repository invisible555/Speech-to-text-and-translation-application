using ReactProject.Server.DTO;
using ReactProject.Server.Model;
using System.Security.Claims;

namespace ReactProject.Server.Services
{
    public interface IUserService
    {
        Task<AuthenticationResultDTO?> Authenticate(string login, string password);
        Task<RegisterResultDTO> RegisterUser(RegisterRequestDTO model);
        Task<bool> LogoutAsync(int userId);


        Task SaveUserTokensAsync(int userId, string accessToken, string refreshToken);
        Task SaveRefreshTokenAsync(int userId, string refreshToken);
        Task SaveAccessTokenAsync(int userId, string accessToken);
        Task<string?> GenerateAccessToken(string refreshToken);
        string? GetUserRole(ClaimsPrincipal user);
        string? GetUserId(ClaimsPrincipal user);
        string? GetUserLogin(ClaimsPrincipal user);
    }
}
