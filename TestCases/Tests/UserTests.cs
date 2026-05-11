using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common.Seeders;
using TestCases.Common;

namespace TestCases.Tests;

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
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

        // Act
        seeder.SeedAdmin(scope);

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
        seeder.SeedAdmin(scope);
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
        seeder.SeedAdmin(scope);
        
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

    [Fact]
    public void Update_User()
    {
        var provider = TestServices.Create();
        var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var usersService = scope.ServiceProvider.GetRequiredService<UserService>();

        //Act
        seeder.SeedAdmin(scope);

        if(!usersService.UserExists("update@mail.com"))
        {
            usersService.addUser(new RegisterDto
            {
                Name = "update",
                Email = "update@mail.com",
                Password = "update@123",
                RoleId = 2
            });
        }

        //Get User
        var allUsers = usersService.getAllUsers(new UserFilters { Name = "update" });
        var user = allUsers.Data?.FirstOrDefault();

        Assert.NotNull(user);
        Assert.NotNull(user.Id);

        var response = usersService.updateUser((int)user.Id, new UpdateUserDto { IsActive = false });

        //Asserts
        Assert.Equal(200, response.Code);

        Assert.NotNull(response.Data);

        var updatedUser = usersService.getAllUsers(new UserFilters { Name = "update" })
                                       .Data?.FirstOrDefault();

        Assert.NotNull(updatedUser);

        Assert.False(updatedUser.IsActive);
    }

    [Fact]
    public void Delete_User()
    {
        var provider = TestServices.Create();
        var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var usersService = scope.ServiceProvider.GetRequiredService<UserService>();

        //Act
        seeder.SeedAdmin(scope);

        if (!usersService.UserExists("update@mail.com"))
        {
            usersService.addUser(new RegisterDto
            {
                Name = "update",
                Email = "update@mail.com",
                Password = "update@123",
                RoleId = 2
            });
        }

        var allUsers = usersService.getAllUsers(new UserFilters { Name = "update" });
        var user = allUsers.Data?.FirstOrDefault();

        Assert.NotNull(user);
        Assert.NotNull(user.Id);

        var response = usersService.deleteUser((int)user.Id);
        
        Assert.Equal(200, response.Code);

        var deletedUserCheck = usersService.getAllUsers(new UserFilters { Name = "update" })
                                      .Data?
                                      .FirstOrDefault();

        Assert.Null(deletedUserCheck);
    }
}
