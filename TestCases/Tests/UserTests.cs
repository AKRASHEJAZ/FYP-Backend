using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common.Seeders;

namespace TestCases.tests;

public class UserTests
{
    [Fact]
    public void Should_Create_Admin_User()
    {
        // Arrange
        var provider = TestServices.Create();

        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();

        // Act
        seeder.SeedAdmin();

        // Assert
        var userExists = userService.UserExists("admin@example.com");

        Assert.True(userExists);
    }

    [Fact]
    public void Get_Users()
    {
        var provider = TestServices.Create();

        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();

        // Act
        seeder.SeedAdmin();
        var response = userService.getAllUsers(new UserFilters());
        
        //Asserts
        Assert.Equal(200, response.Code);
        
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data);
    }

    [Fact]
    public void Add_User()
    {
        var provider = TestServices.Create();
        var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var usersService = scope.ServiceProvider.GetRequiredService<UserService>();

        //Act
        seeder.SeedAdmin();
        
        usersService.addUser(new RegisterDto
        { 
            Name = "cashier" , 
            Email = "cashier@mail.com",
            Password = "cashier@123",
            RoleId = 2
        });

        //Asserts
        var userExists = usersService.UserExists("cashier@mail.com");
        Assert.True(userExists);
    }
}
