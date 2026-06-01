using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;

namespace TestCases.Tests;

public class InventoryBatchCrudTests
{
    [Fact]
    public async Task InventoryBatch_Create_And_Read_Flow()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var category = new Category { Name = "cat_batch_crud" };
            var unit = new Unit { Name = "unit_batch_crud", Symbol = "pc" };
            db.Categories.Add(category);
            db.Units.Add(unit);
            await db.SaveChangesAsync();

            var product = new Product
            {
                Name = "batchprod",
                CategoryId = category.Id,
                UnitId = unit.Id,
                IsActive = true,
                IsSellable = true,
                IsPurchasable = true,
                DoesExpire = true
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var mfgDate = new DateOnly(2025, 6, 1);
            var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));

            // Create
            var createResponse = await batchService.AddInventoryBatchAsync(new AddInventoryBatchDto
            {
                ProductId = product.Id,
                PurchasePrice = 10.50m,
                SellingPrice = 15.00m,
                PurchasedQuantity = 100,
                MFGDate = mfgDate,
                ExpiryDate = expiryDate
            });

            Assert.Equal(200, createResponse.Code);
            Assert.NotNull(createResponse.Data);
            Assert.True(createResponse.Data!.Id > 0);
            Assert.Equal(product.Id, createResponse.Data.ProductId);
            Assert.False(string.IsNullOrWhiteSpace(createResponse.Data.BatchCode));
            Assert.Equal(10.50m, createResponse.Data.PurchasePrice);
            Assert.Equal(15.00m, createResponse.Data.SellingPrice);
            Assert.Equal(mfgDate, createResponse.Data.MFGDate);
            Assert.Equal(expiryDate, createResponse.Data.ExpiryDate);

            var createdId = createResponse.Data.Id;

            // Read (by id)
            var getByIdResponse = await batchService.GetInventoryBatchByIdAsync(createdId);

            Assert.Equal(200, getByIdResponse.Code);
            Assert.NotNull(getByIdResponse.Data);
            Assert.Equal(createResponse.Data.BatchCode, getByIdResponse.Data!.BatchCode);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetAll_Returns_Empty_When_No_Batches()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var response = await batchService.GetAllInventoryBatchesAsync(new InventoryBatchFilters());

            Assert.Equal(400, response.Code);
            Assert.Equal("No Batches Found", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_Add_With_Invalid_ProductId_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var response = await batchService.AddInventoryBatchAsync(new AddInventoryBatchDto
            {
                ProductId = 99999,
                PurchasePrice = 1m,
                SellingPrice = 2m,
                PurchasedQuantity = 1
            });

            Assert.Equal(400, response.Code);
            Assert.Equal("Product not found for the given ProductId.", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetById_When_Not_Found_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var response = await batchService.GetInventoryBatchByIdAsync(99999);

            Assert.Equal(400, response.Code);
            Assert.Equal("Inventory batch not found.", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetAll_Returns_Batches_With_Stock()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var batch = await SeedBatchAsync(db, quantity: 20);
            await SeedInventoryActionAsync(db, batch.Id, InventoryActionType.Sale, InventoryReferenceType.Sale, 4);
            await SeedInventoryActionAsync(db, batch.Id, InventoryActionType.Damage, InventoryReferenceType.Damage, 3);

            var response = await batchService.GetAllInventoryBatchesAsync(new InventoryBatchFilters
            {
                Id = new List<int> { batch.Id }
            });

            Assert.Equal(200, response.Code);
            Assert.NotNull(response.Data);

            var item = Assert.Single(response.Data!.Items);
            Assert.NotNull(item.Stocks);
            Assert.Equal(20, item.Stocks!.Quantity);
            Assert.Equal(4, item.Stocks.Sold);
            Assert.Equal(3, item.Stocks.Damaged);
            Assert.Equal(13, item.Stocks.AvailableStock);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetBatchStock_With_Invalid_Id_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var response = await batchService.GetBatchStockAsync(0);

            Assert.Equal(400, response.Code);
            Assert.Equal("Please Enter a Valid Id", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetBatchStock_When_Batch_Not_Found_Returns_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var response = await batchService.GetBatchStockAsync(99999);

            Assert.Equal(400, response.Code);
            Assert.Equal("No Inventory Batch Found", response.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task InventoryBatch_GetBatchStock_Calculates_Sold_Damaged_And_Returned()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var batchService = scope.ServiceProvider.GetRequiredService<InventoryBatchService>();

        try
        {
            var batch = await SeedBatchAsync(db, quantity: 30);

            await SeedInventoryActionAsync(db, batch.Id, InventoryActionType.Sale, InventoryReferenceType.Sale, 5);
            await SeedInventoryActionAsync(db, batch.Id, InventoryActionType.Damage, InventoryReferenceType.Damage, 4);
            await SeedInventoryActionAsync(db, batch.Id, InventoryActionType.Return, InventoryReferenceType.Return, 2);

            var response = await batchService.GetBatchStockAsync(batch.Id);

            Assert.Equal(200, response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(30, response.Data!.Quantity);
            Assert.Equal(5, response.Data.Sold);
            Assert.Equal(4, response.Data.Damaged);
            Assert.Equal(2, response.Data.Returned);
            Assert.Equal(21, response.Data.AvailableStock);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    private static async Task<InventoryBatch> SeedBatchAsync(AppDbContext db, decimal quantity)
    {
        var category = new Category { Name = $"cat_batch_stock_{Guid.NewGuid():N}" };
        var unit = new Unit { Name = $"unit_batch_stock_{Guid.NewGuid():N}", Symbol = "pc" };

        db.Categories.Add(category);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = $"batch_stock_product_{Guid.NewGuid():N}",
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
            BatchCode = $"batch-stock-{Guid.NewGuid():N}",
            PurchasePrice = 10,
            SellingPrice = 15,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow
        };

        db.InventoryBatches.Add(batch);
        await db.SaveChangesAsync();
        return batch;
    }

    private static async Task SeedInventoryActionAsync(
        AppDbContext db,
        int batchId,
        InventoryActionType actionType,
        InventoryReferenceType referenceType,
        decimal quantity)
    {
        db.InventoryActions.Add(new InventoryAction
        {
            InventoryBatchId = batchId,
            ActionType = actionType,
            ReferenceType = referenceType,
            Quantity = quantity,
            ReferenceId = 1,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
