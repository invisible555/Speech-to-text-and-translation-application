using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using ReactProject.Server.DTO;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
 
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;

    public UserController(IUserService userService, IWebHostEnvironment env)
    {
        _userService = userService;
        _env = env;
      
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO user)
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
        
        return Ok(new
        {
            accessToken = _user.AccessToken,
            refreshToken = _user.RefreshToken,
            login = user.Login,
            tokenExpiredTime = _user.ExpiryTime,
        });
    }
    [AllowAnonymous]
    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO model)
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

        return Ok(new
        {
            message = "Udało się zarejestrować",
        });
    }
    [Authorize(Roles ="admin")]
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin()
    {
        return Ok();
    }

    [Authorize(Roles ="user,admin")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var login = _userService.GetUserLogin(User);
        var role = _userService.GetUserRole(User);
        var email = await _userService.GetUserEmail(User);
        if (login == null || role == null || email == null)
            return NotFound();

        var response = new ProfileRequestDTO
        {
            Login = login,
            Role = role,
            Email = email,

        };

        return Ok(response);
    }
    [Authorize]
    [HttpGet("role")]
    public IActionResult GetRoleByClaim()
    {
       var role = _userService.GetUserRole(User);
        return Ok(new { role });
    }

    [HttpDelete("user")]
    public IActionResult DeleteUser()
    {
        return Ok();
    }
  
    [HttpPost("refresh-access-token")]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshRequestDTO request)
    {
        var accessToken = await _userService.GenerateAccessToken(request.RefreshToken);

        if (accessToken == null)
        {
            return Unauthorized(new { message = "Refresh token jest nieważny lub wygasł." });
        }

        return Ok(new { accessToken });
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdString = _userService.GetUserId(User);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        await _userService.LogoutAsync(userId);
        return Ok();
    }
}
