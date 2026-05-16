using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Entities;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;

namespace TestCases.Tests;

public class InventoryBatchValidationTests
{
    [Fact]
    public async Task InventoryBatch_Add_Inactive_Product_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db, isActive: false);

            var response = await batchService.AddInventoryBatchAsync(ValidBatchDto(product.Id));

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Not_Purchasable_Product_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db, isPurchasable: false);

            var response = await batchService.AddInventoryBatchAsync(ValidBatchDto(product.Id));

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Expiring_Product_Without_Expiry_Date_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db, doesExpire: true);

            var dto = ValidBatchDto(product.Id);
            dto.ExpiryDate = null;

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Expiry_Date_Not_In_Future_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db, doesExpire: true);

            var dto = ValidBatchDto(product.Id);
            dto.ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Expiry_Date_Before_Mfg_Date_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db, doesExpire: true);

            var dto = ValidBatchDto(product.Id);
            dto.MFGDate = new DateOnly(2026, 6, 1);
            dto.ExpiryDate = new DateOnly(2025, 6, 1);

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Purchase_Price_Not_Positive_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db);

            var dto = ValidBatchDto(product.Id);
            dto.PurchasePrice = 0;

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Selling_Price_Not_Positive_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db);

            var dto = ValidBatchDto(product.Id);
            dto.SellingPrice = 0;

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Selling_Price_Less_Than_Purchase_Price_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db);

            var dto = ValidBatchDto(product.Id);
            dto.PurchasePrice = 20m;
            dto.SellingPrice = 10m;

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_Purchased_Quantity_Not_Positive_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var product = await SeedProductAsync(db);

            var dto = ValidBatchDto(product.Id);
            dto.PurchasedQuantity = 0;

            var response = await batchService.AddInventoryBatchAsync(dto);

            Assert.Equal(400, response.Code);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    private static AddInventoryBatchDto ValidBatchDto(int productId) => new()
    {
        ProductId = productId,
        PurchasePrice = 10m,
        SellingPrice = 15m,
        PurchasedQuantity = 1,
        MFGDate = new DateOnly(2025, 6, 1),
        ExpiryDate = new DateOnly(2026, 6, 1)
    };

    private static async Task<Product> SeedProductAsync(
        AppDbContext db,
        bool isActive = true,
        bool isPurchasable = true,
        bool doesExpire = false)
    {
        var category = new Category { Name = "cat_batch_val" };
        var unit = new Unit { Name = "unit_batch_val", Symbol = "pc" };
        db.Categories.Add(category);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = "batch_val_product",
            CategoryId = category.Id,
            UnitId = unit.Id,
            IsActive = isActive,
            IsSellable = true,
            IsPurchasable = isPurchasable,
            DoesExpire = doesExpire
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        return product;
    }
}
