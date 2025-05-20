using Azure.Core;
using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public interface IUserTokenRepository
    {
        Task AddAccessTokenAsync(UserAccessTokens token);
        Task AddRefreshTokenAsync(UserRefreshTokens token);
        Task<UserRefreshTokens?> GetRefreshTokenAsync(string token);
        Task<UserAccessTokens?> GetAccessTokenAsync(string token);
        Task DeactivateAccessTokenAsync(string token);
        Task DeactivateAllTokensAsync(int userId);
        Task SaveChangesAsync();
        Task SaveUserTokensAsync(int userId, string accessToken, DateTime accessExpiry,
                         string refreshToken, DateTime refreshExpiry);
        Task<bool> IsAccessTokenValidAsync(string token);

    }
}
