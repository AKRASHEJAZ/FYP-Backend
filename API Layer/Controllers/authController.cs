using Business_Layer.DTOS.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Endpoint To Register a new user
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns>Registered user JWT token || Error Message</returns>
    [HttpPost("register")]
    public IActionResult Register(RegisterDTO newUser)
    {
        var result = _authService.Register(newUser);
        
        return StatusCode(result.Code,result);
    }

    /// <summary>
    /// Endpoint To LogIn
    /// </summary>
    /// <param name="user"></param>
    /// <returns>User JWT token || Error Message</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO user)
    {
        var result = _authService.Login(user);

        return StatusCode(result.Code, result);
    }

}