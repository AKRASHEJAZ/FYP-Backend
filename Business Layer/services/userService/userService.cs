using Business_Layer.Common;
using Business_Layer.DTOS.User;
using Data_Layer.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class userService
{
    private readonly IUserRepository _userRepo;

    private readonly IHttpContextAccessor _context;

    public userService(IUserRepository userRepo, IHttpContextAccessor context)
    {
        _userRepo = userRepo;
        _context = context;
    }

    public ApiResponse<userDTO> GetAuthUser()
    {
        var userId = _context.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) return ApiResponse<userDTO>.Fail("User not found",404);

        var user =  _userRepo.GetById(int.Parse(userId));

        if(user == null) return ApiResponse<userDTO>.Fail("User not found",404);

        return (ApiResponse<userDTO>.Success(new userDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role.Name,
            },
            "User retrieved successfully",
            200
            )
        );
    }


}

