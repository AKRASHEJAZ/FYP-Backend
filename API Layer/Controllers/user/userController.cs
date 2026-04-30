using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Data_Layer.filters;
using Business_Layer.DTOS;
using Business_Layer.services;

namespace Api_Layer.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }


    /// <summary>
    /// Get Currently Authenticated User Data
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("me")]
    public IActionResult GetAuthUser()
    {
        var result = _userService.GetAuthUser();
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Get All User Data With Filters (Admin Only)
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("get")]
    public IActionResult getAllUsers([FromBody] UserFilters filters)
    {
        var result = _userService.getAllUsers(filters);
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Add A new user (Admin Only)
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("add")]
    public IActionResult addUser([FromBody] RegisterDto user)
    {
        var result = _userService.addUser(user);
        return StatusCode(result.Code, result);
    }

    /// <summary>
    /// Delete a User by User Id (Admin Only)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public IActionResult deleteUser([FromRoute]int id)
    {
        var result = _userService.deleteUser(id);
        return StatusCode(result.Code, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id}")]

    public IActionResult updateUser([FromRoute] int id, [FromBody] UpdateUserDto user)
    {
        var result = _userService.updateUser(id, user);
        return StatusCode(result.Code, result);
    }
}
