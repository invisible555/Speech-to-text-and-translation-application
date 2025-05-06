using Microsoft.IdentityModel.Tokens;
using ReactProject.Server.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtService
{
    private readonly string _jwtSecret;
    private readonly IConfiguration _config;
    private readonly int tokenLiveTime = 15; 

    public JwtService(IConfiguration config)
    {
        _config = config;
        _jwtSecret = config["Jwt:Key"];
    }

    public string GenerateAccessToken(string id,string username, string role)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(tokenLiveTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false // ważne — sprawdzamy wygasły token!
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        return principal;
    }
}
