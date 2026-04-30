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

    public ApiResponse<userDto> GetAuthUser()
    {
        try
        {
            var userId = _context.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return ApiResponse<userDto>.Fail("User not found", 404);

            var user = _userRepo.GetById(int.Parse(userId));

            if (user == null) return ApiResponse<userDto>.Fail("User not found", 404);

            return (ApiResponse<userDto>.Success(new userDto
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
            return ApiResponse<userDto>.Fail("An error occurred while retrieving the user: " + e.Message, 500);
        }

    }

    public ApiResponse<List<userDto>> getAllUsers(UserFilters filters)
    {
        try
        {
            var users = _userRepo.GetAll(filters);
            
            if(users == null || users.Count == 0)
                throw new Exception("No users found matching the provided filters.");

            var userDTOs = users.Select(user => new userDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Role = user.Role.Name,
                RoleId = user.RoleId,
            }).ToList();
            return ApiResponse<List<userDto>>.Success(userDTOs, "Users retrieved successfully", 200);
        }
        catch (Exception e)
        {
            return ApiResponse<List<userDto>>.Fail("An error occurred while retrieving users: " + e.Message, 500);
        }
    }

    public ApiResponse<string?> addUser(RegisterDto user)
    {
        try
        {

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
                return ApiResponse<string?>.Success(null, "User added successfully", 201);
            else
                return ApiResponse<string?>.Fail("Failed to add user", 400);

        }
        catch (Exception e)
        {
            return ApiResponse<string?>.Fail("An error occurred while adding the user: " + e.Message, 500);
        }
    }

    public ApiResponse<userDto> deleteUser(int id)
    {
        try
        {
            var currentUser = this.GetAuthUser();
            if(id == currentUser.Data?.Id)
                return ApiResponse<userDto>.Fail("You cannot delete your own account", 400);

            var user = _userRepo.Delete(id);

            if (user != null)
                return ApiResponse<userDto>.Success(new userDto
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

            else
                return ApiResponse<userDto>.Fail("Failed to delete user OR user not found", 400);
        }
        catch (Exception e)
        {
            return ApiResponse<userDto>.Fail("An error occurred while deleting the user: " + e.Message, 500);
        }
    }

    public ApiResponse<userDto> updateUser(int id, UpdateUserDto updatedUser)
    {
        try
        {
            if (updatedUser != null)
            {

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

                if (user != null)
                    return ApiResponse<userDto>.Success(new userDto
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
                    200
                    );
                else
                    return ApiResponse<userDto>.Fail("Failed to update user OR user not found", 400);
            }
            else {
                return ApiResponse<userDto>.Fail("Invalid user data", 400);
            }
        }
        catch (Exception e)
        {
            return ApiResponse<userDto>.Fail("An error occurred while updating the user: " + e.Message, 500);
        }
    }
}

