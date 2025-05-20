using ReactProject.Server.Repositories;

namespace ReactProject.Server.Services
{
    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly IUserTokenRepository _tokenRepository;


        public TokenValidatorService(IUserTokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }
        public async Task<bool> IsAccessTokenValidAsync(string token)
        {
            return await _tokenRepository.IsAccessTokenValidAsync(token);
        }

    }
}
