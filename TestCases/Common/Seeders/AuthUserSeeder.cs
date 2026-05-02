using Business_Layer.DTOS;
using Business_Layer.services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace TestCases.Common.Seeders;

public class AuthUserSeeder
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public AuthUserSeeder(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    public void SeedAdmin(IServiceScope scope)
    {
        SeedRoles();

        // Check if admin already exists
        if (!_userService.UserExists("admin@example.com"))
        {
            _authService.Register(new RegisterDto
            {
                Name = "Admin User",
                Email = "admin@example.com",
                Password = "Admin@123",
                RoleId = 1
            });
        }

        var httpContextAccessor = scope.ServiceProvider
        .GetRequiredService<IHttpContextAccessor>();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = principal
        };
    }

    public void SeedRoles()
    {
        if (!_userService.RoleExists("Admin"))
            _userService.AddRole("Admin");

        if (!_userService.RoleExists("Cashier"))
            _userService.AddRole("Cashier");
    }
}