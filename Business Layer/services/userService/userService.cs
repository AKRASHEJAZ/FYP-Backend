using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Data_Layer.filters;
using Data_Layer.Entities;
using Business_Layer.helpers;

namespace Business_Layer.services;

public class UserService
{
    private readonly IUserRepository _userRepo;

    private readonly IHttpContextAccessor _context;

    public UserService(IUserRepository userRepo, IHttpContextAccessor context)
    {
        _userRepo = userRepo;
        _context = context;
    }

    public ApiResponse<UserDto> GetAuthUser()
    {
        try
        {
            
            var userId = _context.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return ApiResponse<UserDto>.Fail("User not found", 404);

            var user = _userRepo.GetById(int.Parse(userId));

            if (user == null) return ApiResponse<UserDto>.Fail("User not found", 404);

            return (ApiResponse<UserDto>.Success(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role.Name,
                RoleId = user.RoleId,
            },
                "User retrieved successfully",
                200
                )
            );
        }
        catch (Exception e)
        {
            return ApiResponse<UserDto>.Fail("An error occurred while retrieving the user: " + e.Message, 500);
        }
    }

    public ApiResponse<List<UserDto>> getAllUsers(UserFilters filters)
    {
        try
        {
            var users = _userRepo.GetAll(filters);
            
            if(users == null || users.Count == 0)
                return ApiResponse<List<UserDto>>.Fail("No users found matching the provided filters", 404);

            var userDTOs = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role.Name,
                RoleId = user.RoleId,
            }).ToList();
            return ApiResponse<List<UserDto>>.Success(userDTOs, "Users retrieved successfully", 200);
        }
        catch (Exception e)
        {
            return ApiResponse<List<UserDto>>.Fail("An error occurred while retrieving users: " + e.Message, 500);
        }
    }

    public ApiResponse<string?> addUser(RegisterDto user)
    {
        try
        {
            var authResult = this.GetAuthUser();

            if (authResult.Code != 200 || authResult.Data == null)
                return ApiResponse<string?>.Fail("Unauthorized", 401);

            var authUser = authResult.Data;

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow,
                RoleId = user.RoleId,
                IsActive = true,
                PasswordHash = PasswordHelper.HashPassword(user.Password),
            };

            if (_userRepo.Add(newUser))
            {

                _userRepo.CreateUserAuditLog(new UserAuditLog
                {
                    UserId = newUser.Id,
                    Action = "Created",
                    Details = "User Created",
                    PerformedBy = authUser.Id!

                });
                return ApiResponse<string?>.Success(null, "User added successfully", 201);
            }
   
            else
                return ApiResponse<string?>.Fail("Failed to add user", 400);

        }
        catch (Exception e)
        {
            return ApiResponse<string?>.Fail("An error occurred while adding the user: " + e.Message, 500);
        }
    }

    public ApiResponse<UserDto> deleteUser(int id)
    {
        try
        {
            var authResult = this.GetAuthUser();

            if (authResult.Code != 200 || authResult.Data == null)
                return ApiResponse<UserDto>.Fail("Unauthorized", 401);

            var authUser = authResult.Data;

            if(id == authUser?.Id)
                return ApiResponse<UserDto>.Fail("You cannot delete your own account", 400);

            var user = _userRepo.Delete(id);

            if (user != null)
            {
                return ApiResponse<UserDto>.Success(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Role = user.Role.Name,
                    RoleId = user.RoleId,
                },
                "User deleted successfully",
                200
                );
            }

            else
                return ApiResponse<UserDto>.Fail("Failed to delete user OR user not found", 400);
        }
        catch (Exception e)
        {
            return ApiResponse<UserDto>.Fail("An error occurred while deleting the user: " + e.Message, 500);
        }
    }

    public ApiResponse<UserDto> updateUser(int id, UpdateUserDto updatedUser)
    {
        try
        {
            if (updatedUser == null)
                return ApiResponse<UserDto>.Fail("Invalid user data", 400);

            var authResult = GetAuthUser();

            if (authResult.Code != 200 || authResult.Data == null)
                return ApiResponse<UserDto>.Fail("Unauthorized", 401);

            var authUser = authResult.Data;

            var newUser = new User();

            if (updatedUser.Name != null)
                newUser.Name = updatedUser.Name;

            if (updatedUser.Email != null)
                newUser.Email = updatedUser.Email;

            if (updatedUser.IsActive.HasValue)
                newUser.IsActive = updatedUser.IsActive.Value;

            if (updatedUser.RoleId.HasValue)
                newUser.RoleId = updatedUser.RoleId.Value;

            var user = _userRepo.Update(id, newUser);

            if (user == null)
                return ApiResponse<UserDto>.Fail("Failed to update user OR user not found", 400);

            _userRepo.CreateUserAuditLog(new UserAuditLog
            {
                UserId = user.Id,
                Action = "Updated",
                Details = "User Updated",
                PerformedBy = authUser?.Id!
            });

            return ApiResponse<UserDto>.Success(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role.Name,
                RoleId = user.RoleId,
            },
            "User updated successfully",
            200);
        }
        catch (Exception e)
        {
            return ApiResponse<UserDto>.Fail(
                "An error occurred while updating the user: " + e.Message, 500);
        }
    }

    /*
     * THESE FOLLOWING METHODS ARE NOT EXPOSED TO THE CONTROLLER, 
     * THEY ARE USED INTERNALLY BY THE SERVICE TO CHECK FOR EXISTENCE OF USERS AND ROLES AND TO ADD ROLES
     * USEFUL FOR SEEDING THE DATABASE WITH ROLES AND CHECKING FOR EXISTENCE OF USERS AND ROLES BEFORE PERFORMING CERTAIN ACTIONS
     */
    public void AddRole(string roleName)
    {
        var role = new Role { Name = roleName };
        _userRepo.AddRole(role);
    }

    public bool UserExists(string user)
    {
        return _userRepo.UserExists(user);
    }

    public bool RoleExists(string role)
    {
        return _userRepo.RoleExists(role);
    }
}

