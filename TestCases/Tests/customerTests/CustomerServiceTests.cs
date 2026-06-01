using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;

namespace TestCases.Tests;

public class CustomerServiceTests
{
    [Fact]
    public async Task Customer_Create_And_Read_Flow()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customerService = scope.ServiceProvider.GetRequiredService<CustomerService>();

        try
        {
            var createResponse = await customerService.CreateCustomerAsync(new CreateCustomerDto
            {
                Name = "Walk In Customer",
                Phone = "03000000000",
                Email = "walkin@test.com",
                Address = "Test address",
                IsWalkIn = true
            });

            Assert.Equal(200, createResponse.Code);
            Assert.NotNull(createResponse.Data);
            Assert.True(createResponse.Data!.Id > 0);
            Assert.True(createResponse.Data.IsWalkIn);

            var getByIdResponse = await customerService.GetCustomerByIdAsync(createResponse.Data.Id);

            Assert.Equal(200, getByIdResponse.Code);
            Assert.NotNull(getByIdResponse.Data);
            Assert.Equal("Walk In Customer", getByIdResponse.Data!.Name);

            var getAllResponse = await customerService.GetCustomersAsync(new CustomerFilters
            {
                Name = "Walk",
            });

            Assert.Equal(200, getAllResponse.Code);
            Assert.NotNull(getAllResponse.Data);
            Assert.Single(getAllResponse.Data!.Items);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Customer_Create_With_Empty_Name_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customerService = scope.ServiceProvider.GetRequiredService<CustomerService>();

        try
        {
            var emptyName = await customerService.CreateCustomerAsync(new CreateCustomerDto { Name = "" });
            Assert.Equal(400, emptyName.Code);
            Assert.Equal("Customer name is required.", emptyName.Message);

            var whitespaceName = await customerService.CreateCustomerAsync(new CreateCustomerDto { Name = "   " });
            Assert.Equal(400, whitespaceName.Code);
            Assert.Equal("Customer name is required.", whitespaceName.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Customer_GetById_When_Not_Found_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customerService = scope.ServiceProvider.GetRequiredService<CustomerService>();

        try
        {
            var response = await customerService.GetCustomerByIdAsync(99999);

            Assert.Equal(400, response.Code);
            Assert.Equal("Customer not found.", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }
}
