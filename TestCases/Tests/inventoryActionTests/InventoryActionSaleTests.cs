using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;
using TestCases.Common.Seeders;

namespace TestCases.Tests;

public class InventoryActionSaleTests
{
    [Fact]
    public async Task Sale_Create_Succeeds_And_Updates_Batch_Stock()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);
            var batch = await SeedBatchAsync(db, quantity: 10);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 3
                    }
                }
            });

            Assert.Equal(200, response.Code);
            Assert.Equal("Sale created successfully", response.Data);

            var sale = await db.Sales.SingleAsync();
            var action = await db.InventoryActions.SingleAsync();

            Assert.Equal(sale.Id, action.ReferenceId);
            Assert.Equal(InventoryReferenceType.Sale, action.ReferenceType);
            Assert.Equal(InventoryActionType.Sale, action.ActionType);
            Assert.Equal(3, action.Quantity);

            var stockResponse = await batchService.GetBatchStockAsync(batch.Id);

            Assert.Equal(200, stockResponse.Code);
            Assert.NotNull(stockResponse.Data);
            Assert.Equal(10, stockResponse.Data!.Quantity);
            Assert.Equal(3, stockResponse.Data.Sold);
            Assert.Equal(7, stockResponse.Data.AvailableStock);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Sale_Create_Insufficient_Stock_Returns_Error_And_Does_Not_Persist()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);
            var batch = await SeedBatchAsync(db, quantity: 2);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 3
                    }
                }
            });

            Assert.Equal(400, response.Code);
            Assert.Contains("Insufficient stock", response.Message);
            Assert.Empty(db.Sales);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Sale_Create_With_Empty_Actions_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>()
            });

            Assert.Equal(400, response.Code);
            Assert.Equal("Please Specify any action", response.Message);
            Assert.Empty(db.Sales);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Sale_Create_With_Invalid_Batch_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = 99999,
                        Quantity = 1
                    }
                }
            });

            Assert.Equal(400, response.Code);
            Assert.Equal("Invalid Batch Id Entered", response.Message);
            Assert.Empty(db.Sales);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Sale_Create_With_Non_Positive_Quantity_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);
            var batch = await SeedBatchAsync(db, quantity: 10);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 0
                    }
                }
            });

            Assert.Equal(400, response.Code);
            Assert.Equal("Quantity must be greater than zero.", response.Message);
            Assert.Empty(db.Sales);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Sale_Create_Without_Authenticated_User_Returns_Unauthorized()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            var customer = await SeedCustomerAsync(db);
            var batch = await SeedBatchAsync(db, quantity: 10);

            var response = await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 1
                    }
                }
            });

            Assert.Equal(401, response.Code);
            Assert.Equal("Unauthorized", response.Message);
            Assert.Empty(db.Sales);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task GetSales_Returns_Created_Sale_With_Actions()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var customer = await SeedCustomerAsync(db);
            var batch = await SeedBatchAsync(db, quantity: 10);

            await actionService.CreateSaleAsync(new AddSaleDto
            {
                CustomerId = customer.Id,
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 2
                    }
                }
            });

            var response = await actionService.GetSalesAsync(new SaleFilters
            {
                CustomerId = customer.Id,
                IsIncludeCustomer = true,
                IsIncludeUser = true,
                IsIncludeInventoryBatch = true
            });

            Assert.Equal(200, response.Code);
            Assert.NotNull(response.Data);
            var sale = Assert.Single(response.Data!.Items);
            Assert.Equal(customer.Id, sale.CustomerId);
            Assert.NotNull(sale.Customer);
            Assert.Equal("Test Customer", sale.Customer.Name);

            var action = Assert.Single(sale.Actions);
            Assert.Equal(batch.Id, action.InventoryBatchId);
            Assert.Equal(2, action.Quantity);
            Assert.NotNull(action.InventoryBatch);
            Assert.Equal(batch.BatchCode, action.InventoryBatch.BatchCode);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task GetSales_When_No_Sales_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            var response = await actionService.GetSalesAsync(new SaleFilters());

            Assert.Equal(400, response.Code);
            Assert.Equal("No sale found", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    private static async Task<Customer> SeedCustomerAsync(AppDbContext db)
    {
        var customer = new Customer
        {
            Name = "Test Customer",
            Email = "customer@test.com",
            Phone = "03000000000",
            CreatedAt = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private static async Task<InventoryBatch> SeedBatchAsync(AppDbContext db, decimal quantity)
    {
        var category = new Category { Name = $"cat_sale_{Guid.NewGuid():N}" };
        var unit = new Unit { Name = $"unit_sale_{Guid.NewGuid():N}", Symbol = "pc" };

        db.Categories.Add(category);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = $"sale_product_{Guid.NewGuid():N}",
            CategoryId = category.Id,
            UnitId = unit.Id,
            IsActive = true,
            IsSellable = true,
            IsPurchasable = true,
            DoesExpire = false
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var batch = new InventoryBatch
        {
            ProductId = product.Id,
            BatchCode = $"batch-{Guid.NewGuid():N}",
            PurchasePrice = 10,
            SellingPrice = 15,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow
        };

        db.InventoryBatches.Add(batch);
        await db.SaveChangesAsync();
        return batch;
    }
}
