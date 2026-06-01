using Business_Layer.helpers;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Interfaces;
using Data_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
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
            ["Jwt:Key"] = "test-secret-key-for-unit-tests-must-be-long-enough",
            ["Jwt:Issuer"] = "test",
            ["Jwt:Audience"] = "test"
        })
        .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddDbContext<AppDbContext>(options =>
            options
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepo>();
        services.AddScoped<IProductRepository, ProductRepo>();
        services.AddScoped<IInventoryBatchRepository, InventoryBatchRepo>();
        services.AddScoped<IInventoryActionRepository, InventoryActionRepo>();
        services.AddScoped<ICustomerRepository, CustomerRepo>();

        // Services
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();
        services.AddScoped<UnitService>();
        services.AddScoped<ProductService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<InventoryBatchService>();
        services.AddScoped<InventoryActionService>();
        services.AddScoped<CustomerService>();

        // Seeder
        services.AddScoped<AuthUserSeeder>();

        // Http context (if needed)
        services.AddHttpContextAccessor();

        return services.BuildServiceProvider();
    }
}