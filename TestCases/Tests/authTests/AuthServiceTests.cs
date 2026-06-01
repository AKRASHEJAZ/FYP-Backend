using Business_Layer.DTOS;
using Business_Layer.services;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;
using TestCases.Common.Seeders;

namespace TestCases.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Login_With_Valid_Credentials_Returns_Token()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        seeder.SeedAdmin(scope);

        var response = authService.Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "Admin@123"
        });

        Assert.Equal(200, response.Code);
        Assert.False(string.IsNullOrWhiteSpace(response.Data));
    }

    [Fact]
    public void Login_With_Wrong_Password_Returns_Unauthorized()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        seeder.SeedAdmin(scope);

        var response = authService.Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "wrong-password"
        });

        Assert.Equal(401, response.Code);
        Assert.Equal("Invalid credentials", response.Message);
    }

    [Fact]
    public void Login_With_Missing_User_Returns_NotFound()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        var response = authService.Login(new LoginDto
        {
            Email = "missing@example.com",
            Password = "Admin@123"
        });

        Assert.Equal(404, response.Code);
        Assert.Equal("User does not exist", response.Message);
    }

    [Fact]
    public void Register_Duplicate_User_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        seeder.SeedAdmin(scope);

        var response = authService.Register(new RegisterDto
        {
            Name = "Admin User",
            Email = "admin@example.com",
            Password = "Admin@123",
            RoleId = 1
        });

        Assert.Equal(400, response.Code);
        Assert.Equal("User already exists", response.Message);
    }
}
