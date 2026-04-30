using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class userController : Controller
{
    private readonly userService _userService;

    public userController(userService userService)
    {
        _userService = userService;
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult GetAuthUser()
    {
        var result = _userService.GetAuthUser();
        return StatusCode(result.Code, result);
    }
}
