using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        
        return Ok(new
        {
            accessToken = _user.AccessToken,
            login = user.Login,
            tokenExpiredTime = _user.ExpiryTime,
        });
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
    {
       /* if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }*/

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

    [Authorize(Roles ="user")]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var username = User.Identity?.Name;
        return Ok(new { username });
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
}
