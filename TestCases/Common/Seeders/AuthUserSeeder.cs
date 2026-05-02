using Business_Layer.DTOS;
using Business_Layer.services;

namespace TestCases.Common.Seeders;

public class AuthUserSeeder
{
    private readonly UserService _userService;

    public AuthUserSeeder(UserService userService)
    {
        _userService = userService;
    }

    public void SeedAdmin()
    {
        SeedRoles();

        // Check if admin already exists
        if (!_userService.UserExists("admin@example.com"))
        {
            _userService.addUser(new RegisterDto
            {
                Name = "Admin User",
                Email = "admin@example.com",
                Password = "Admin@123",
                RoleId = 1
            });
        }
    }

    public void SeedRoles()
    {
        if (!_userService.RoleExists("Admin"))
            _userService.AddRole("Admin");

        if (!_userService.RoleExists("Cashier"))
            _userService.AddRole("Cashier");
    }
}