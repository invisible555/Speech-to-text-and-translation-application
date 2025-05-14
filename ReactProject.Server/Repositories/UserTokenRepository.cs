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
          return  await _dbContext.UserRefreshTokens.FirstOrDefaultAsync(t => t.Token == token && t.IsActive);
        }

        public async Task<UserAccessTokens?> GetAccessTokenAsync(string token)
        {
           return await _dbContext.UserAccessTokens.FirstOrDefaultAsync(t => t.Token == token && t.IsActive);   
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
    }
}
