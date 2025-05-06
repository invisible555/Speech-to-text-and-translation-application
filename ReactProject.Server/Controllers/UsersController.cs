using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactProject.Server.DTO;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;
    private readonly int _accessTokenLiveTime = 15;
    private readonly int _refreshTokenLiveTime = 1;


    public UsersController(IUserService userService, JwtService jwtService, IWebHostEnvironment env)
    {
        _userService = userService;
        _jwtService = jwtService;
        _env = env;
      
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var _user = await _userService.Authenticate(user.Login, user.Password);
        if (_user.Success == false)
        {
            return Unauthorized("Nieprawidłowy login lub hasło");
        }
        
        var role = _user.User.Role;
        if(role == null)
        {
            return Unauthorized("Błąd roli");
        }
        var id = _userService.GetUserId(User);
        var accessToken = _jwtService.GenerateAccessToken(id,user.Login, role);
        var refreshToken = _jwtService.GenerateRefreshToken();

        if (_user.User.Id == null || accessToken == null || refreshToken == null)
        {
            return  Unauthorized("Błąd");
        }
        try
        {
            await _userService.SaveUserTokens(_user.User.Id, accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Błąd w zapisie tokenów");
        }
        return Ok(new
        {
            _accessToken = accessToken,
            _login = user.Login,
            _tokenExpiredTime = accessToken
        });
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.RegisterUser(model);

        if (!result.Success || result.User == null)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        // Tworzenie katalogu użytkownika we wwwroot/UserData/{Login}
        var userDir = Path.Combine(_env.WebRootPath, "UserData", result.User.Login);

        if (!Directory.Exists(userDir))
        {
            Directory.CreateDirectory(userDir);
        }


        return Ok(new
        {
            message = "Udało się zalogować",
        });
    }
    [Authorize]
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin()
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("getprofile")]
    public IActionResult GetProfile()
    {
        var username = User.Identity?.Name;
        return Ok(new { username });
    }
    [Authorize]
    [HttpGet("getrole")]
    public IActionResult GetRoleByClaim()
    {
        var role = _userService.GetUserRole(User);
        return Ok(new { role });
    }


    public IActionResult DeleteUser()
    {

        return Ok();
    }
}
