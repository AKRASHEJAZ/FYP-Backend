using Business_Layer.helpers;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Interfaces;
using Data_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestCases.Common.Seeders;
using Microsoft.Extensions.Configuration;

namespace TestCases.Common;
public static class TestServices
{
    public static ServiceProvider Create()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "test-secret-key",
            ["Jwt:Issuer"] = "test",
            ["Jwt:Audience"] = "test"
        })
        .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Repositories
        services.AddScoped<IUserRepository, UserRepo>();

        // Services
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();

        // Seeder
        services.AddScoped<AuthUserSeeder>();

        // Http context (if needed)
        services.AddHttpContextAccessor();

        return services.BuildServiceProvider();
    }
}