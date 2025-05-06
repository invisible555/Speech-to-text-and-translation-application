using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ReactProject.Server.Entities;

namespace ReactProject.Server.Repositories
{
    public class UserTokenRepository :IUserTokenRepository
    {
        private readonly AppDbContext _context;

        public UserTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAccessTokenAsync(UserAccessTokens token)
        {
            await  _context.UserAccessTokens.AddAsync(token);
        }

        public async Task AddRefreshTokenAsync(UserRefreshTokens token)
        {
            await _context.UserRefreshTokens.AddAsync(token);
        }

        public async Task<UserRefreshTokens?> GetRefreshTokenAsync(string token)
        {
          return  await _context.UserRefreshTokens.FirstOrDefaultAsync(t => t.Token == token && t.IsActive);
        }

        public async Task<UserAccessTokens?> GetAccessTokenAsync(string token)
        {
           return await _context.UserAccessTokens.FirstOrDefaultAsync(t => t.Token == token && t.IsActive);   
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
            var tokens = await _context.UserAccessTokens.Where(t => t.UserId == userId).ToListAsync();
            foreach (var t in tokens)
            {
                t.IsRevoked = true;
            }

            var refresh = await _context.UserRefreshTokens.Where(t => t.UserId == userId).ToListAsync();
            foreach (var t in refresh)
            {
                t.IsRevoked = true;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
