using Microsoft.AspNetCore.Identity;
using ReactProject.Server.DTO;
using ReactProject.Server.Entities;
using ReactProject.Server.Model;
using ReactProject.Server.Repositories;
using System.Security.Claims;

namespace ReactProject.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IUserStorageService _userStorageService;
        private readonly int _accessTokenLiveTime = 15;
        private readonly int _refreshTokenLiveTime = 1;

        public UserService(IUserRepository userRepository, IUserTokenRepository userTokenRepository, JwtService jwtService, IUserStorageService userStorageService)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
            _userStorageService = userStorageService;
        }

        public async Task<AuthenticationResult?> Authenticate(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return new AuthenticationResult { Success = false, ErrorMessage = "Nieprawidłowy login lub hasło." }; ;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result != PasswordVerificationResult.Success)
            {
                return new AuthenticationResult { Success = false, ErrorMessage = "Nieprawidłowy login lub hasło." };
            }

            await _userTokenRepository.DeactivateAllTokensAsync(user.Id);

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(),user.Login, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var accessTokenEntity = new UserAccessTokens
            {
                UserId = user.Id,
                Token = accessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime),
            };

            var refreshTokenEntity = new UserRefreshTokens
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenLiveTime),
            };
            await _userTokenRepository.AddAccessTokenAsync(accessTokenEntity);
            await _userTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
            await _userTokenRepository.SaveChangesAsync();
            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                ExpiryTime = accessTokenEntity.ExpiryDate,
                User = user
            };
        }


        public async Task<RegisterResult> RegisterUser(RegisterRequest model)
        {
            if (await _userRepository.GetByLoginAsync(model.Login) != null)
            {
                return new RegisterResult { Success = false, ErrorMessage = "Login jest zajęty." };
            }

            if (await _userRepository.GetByEmailAsync(model.Email) != null)
            {
                return new RegisterResult { Success = false, ErrorMessage = "Email jest już używany." };
            }

            var user = new User
            {
                Login = model.Login,
                Email = model.Email,
                Password = model.Password,
                Role = "user"
            };
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            try
            {
                await _userRepository.AddUserAsync(user);
                await _userRepository.SaveChangesAsync();
                await _userStorageService.CreateUserDirectoryAsync(user.Login);
                return new RegisterResult { Success = true, User = user };
            }
            catch
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Wystąpił błąd podczas rejestracji."
                };
            }
        }
        public async Task<bool> LogoutAsync(int userId)
        {
            await _userTokenRepository.DeactivateAllTokensAsync(userId);
            await _userTokenRepository.SaveChangesAsync();
            return true;
        }
        //?TODO
        public  string? GetUserRole(ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            return role;
        }
        //?TODO
        public string? GetUserId(ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return id;
        }
        

        public async Task SaveUserTokens(int userId, string accessToken, string refreshToken)
        {
            var accessTokenEntity = new UserAccessTokens
            {
                UserId = userId,
                Token = accessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime),
                IsRevoked = false,
            };

            var refreshTokenEntity = new UserRefreshTokens
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenLiveTime),
                IsRevoked = false,

            };
            try
            {
                await _userTokenRepository.AddAccessTokenAsync(accessTokenEntity);
                await _userTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
                await _userTokenRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Nie udało się zapisać tokenów");
            }
        }


    }
}
