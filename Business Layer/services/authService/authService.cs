using Business_Layer.Common;
using Business_Layer.DTOS.Auth;
using Data_Layer.Entities;
using Data_Layer.Interfaces;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly TokenService _tokenService;

    public AuthService(IUserRepository userRepo, TokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }

    public ApiResponse<string> Register(RegisterDTO dto)
    {
        var existingUser = _userRepo.GetByEmail(dto.Email);

        if (existingUser != null)
        {
            return ApiResponse<string>.Fail("User already exists", 400);
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            RoleId = dto.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _userRepo.Add(user);

        var token = _tokenService.CreateToken(user);

        return ApiResponse<string>.Success(token, "User registered successfully");
    }

    public ApiResponse<string> Login(LoginDTO input)
    {
        var user = _userRepo.GetByEmail(input.Email);

        if (user == null)
        {
            return ApiResponse<string>.Fail("User does not exist", 404);
        }

        if (!PasswordHelper.VerifyPassword(input.Password, user.PasswordHash))
        {
            return ApiResponse<string>.Fail("Invalid credentials", 401);
        }

        var token = _tokenService.CreateToken(user);

        return ApiResponse<string>.Success(token, "Login successful");
    }
}