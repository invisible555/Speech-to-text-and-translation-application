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
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IUserStorageService _userStorageService;
        private readonly int _accessTokenLiveTime;
        private readonly int _refreshTokenLiveTime;

        public UserService(IUserRepository userRepository, IUserTokenRepository userTokenRepository, JwtService jwtService, IUserStorageService userStorageService, IConfiguration config)
        {
            _config = config;
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
            _userStorageService = userStorageService;
            _accessTokenLiveTime = int.TryParse(config["Jwt:AccessTokenLifetimeMinutes"], out var minutes) ? minutes : 15;
            _refreshTokenLiveTime = int.TryParse(config["Jwt:RefreshTokenLifetimeDays"], out var days) ? days : 15;
            _config = config;
        }

        public async Task<AuthenticationResultDTO?> Authenticate(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return new AuthenticationResultDTO { Success = false, ErrorMessage = "Nieprawidłowy login lub hasło." }; ;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result != PasswordVerificationResult.Success)
            {
                return new AuthenticationResultDTO { Success = false, ErrorMessage = "Nieprawidłowy login lub hasło." };
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
            return new AuthenticationResultDTO
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiryTime = accessTokenEntity.ExpiryDate,
                User = user
            };
        }


        public async Task<RegisterResultDTO> RegisterUser(RegisterRequestDTO model)
        {
            if (await _userRepository.GetByLoginAsync(model.Login) != null)
            {
                return new RegisterResultDTO { Success = false, ErrorMessage = "Login jest zajęty." };
            }

            if (await _userRepository.GetByEmailAsync(model.Email) != null)
            {
                return new RegisterResultDTO { Success = false, ErrorMessage = "Email jest już używany." };
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
                return new RegisterResultDTO { Success = true, User = user };
            }
            catch
            {
                return new RegisterResultDTO
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

        public async Task SaveAccessTokenAsync(int userId, string accessToken)
        {
            var accessTokenEntity = new UserAccessTokens
            {
                UserId = userId,
                Token = accessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime),
                IsRevoked = false,
            };

            try
            {
                await _userTokenRepository.AddAccessTokenAsync(accessTokenEntity);
                await _userTokenRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Nie udało się zapisać access tokena.");
            }
        }
        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            var refreshTokenEntity = new UserRefreshTokens
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_refreshTokenLiveTime),
                IsRevoked = false,
            };

            try
            {
                await _userTokenRepository.AddRefreshTokenAsync(refreshTokenEntity);
                await _userTokenRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Nie udało się zapisać refresh tokena.");
            }
        }
        public string? GetUserRole(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        public string? GetUserId(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public string? GetUserLogin(ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
        public async Task<string?> GetUserEmail(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (userId == null)
                return null;
            var userFromDatabase = await _userRepository.GetByIdAsync(int.Parse(userId));
            if (userFromDatabase == null)
                return null;

            return userFromDatabase.Email;
        }


        public async Task SaveUserTokensAsync(int userId, string accessToken, string refreshToken)
        {
            var accessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime);
            var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenLiveTime);

            await _userTokenRepository.SaveUserTokensAsync(userId, accessToken, accessExpiry, refreshToken, refreshExpiry);
        }
        public async Task<string?> GenerateAccessToken(string refreshToken)
        {
            var storedToken = await _userTokenRepository.GetRefreshTokenAsync(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || !storedToken.IsActive)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return null;
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Login, user.Role);

            var tokenEntity = new UserAccessTokens
            {
                UserId = user.Id,
                Token = newAccessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_accessTokenLiveTime),
            };

            await _userTokenRepository.AddAccessTokenAsync(tokenEntity);
            await _userTokenRepository.SaveChangesAsync();

            return newAccessToken;
        }




    }
}
