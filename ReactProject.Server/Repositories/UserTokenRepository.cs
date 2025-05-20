using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public class UserTokenRepository :IUserTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public UserTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAccessTokenAsync(UserAccessTokens token)
        {
            await  _dbContext.UserAccessTokens.AddAsync(token);
        }

        public async Task AddRefreshTokenAsync(UserRefreshTokens token)
        {
            await _dbContext.UserRefreshTokens.AddAsync(token);
        }

        public async Task<UserRefreshTokens?> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<UserAccessTokens?> GetAccessTokenAsync(string token)
        {
            return await _dbContext.UserAccessTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiryDate > DateTime.UtcNow);
        }


        public async Task DeactivateAccessTokenAsync(string token)
        {
            var tokenEntity = await GetAccessTokenAsync(token);
            if (tokenEntity != null)
            {
                tokenEntity.IsRevoked = true;
            }
        }

        public async Task DeactivateAllTokensAsync(int userId)
        {
            var tokens = await _dbContext.UserAccessTokens.Where(t => t.UserId == userId).ToListAsync();
            foreach (var t in tokens)
            {
                t.IsRevoked = true;
            }

            var refresh = await _dbContext.UserRefreshTokens.Where(t => t.UserId == userId).ToListAsync();
            foreach (var t in refresh)
            {
                t.IsRevoked = true;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        public async Task SaveUserTokensAsync(int userId, string accessToken, DateTime accessExpiry,
                                      string refreshToken, DateTime refreshExpiry)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var accessTokenEntity = new UserAccessTokens
                {
                    UserId = userId,
                    Token = accessToken,
                    ExpiryDate = accessExpiry,
                    IsRevoked = false
                };

                var refreshTokenEntity = new UserRefreshTokens
                {
                    UserId = userId,
                    Token = refreshToken,
                    ExpiryDate = refreshExpiry,
                    IsRevoked = false
                };

                await _dbContext.UserAccessTokens.AddAsync(accessTokenEntity);
                await _dbContext.UserRefreshTokens.AddAsync(refreshTokenEntity);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("Nie udało się zapisać tokenów (transakcja nie powiodła się).");
            }
        }
        public async Task<bool> IsAccessTokenValidAsync(string token)
        {
            var dbToken = await _dbContext.UserAccessTokens
                .Where(t => t.Token == token)
                .FirstOrDefaultAsync();

            return dbToken?.IsActive == true;
        }

    }
}
