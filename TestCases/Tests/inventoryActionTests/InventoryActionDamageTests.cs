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

public class InventoryActionDamageTests
{
    [Fact]
    public async Task Damage_Create_Succeeds_And_Updates_Batch_Stock()
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
            var batch = await SeedBatchAsync(db, quantity: 10);

            var response = await actionService.CreateDamageAsync(new AddDamageDto
            {
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 2,
                        Notes = "Broken during handling"
                    }
                }
            });

            Assert.Equal(200, response.Code);
            Assert.Equal("Damage record created successfully", response.Data);

            var damage = await db.Damages.SingleAsync();
            var action = await db.InventoryActions.SingleAsync();

            Assert.Equal(damage.Id, action.ReferenceId);
            Assert.Equal(InventoryReferenceType.Damage, action.ReferenceType);
            Assert.Equal(InventoryActionType.Damage, action.ActionType);
            Assert.Equal("Broken during handling", action.Notes);

            var stockResponse = await batchService.GetBatchStockAsync(batch.Id);

            Assert.Equal(200, stockResponse.Code);
            Assert.NotNull(stockResponse.Data);
            Assert.Equal(2, stockResponse.Data!.Damaged);
            Assert.Equal(8, stockResponse.Data.AvailableStock);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Damage_Create_Without_Notes_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var batch = await SeedBatchAsync(db, quantity: 10);

            var response = await actionService.CreateDamageAsync(new AddDamageDto
            {
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 1,
                        Notes = "   "
                    }
                }
            });

            Assert.Equal(400, response.Code);
            Assert.Equal("Notes are required for damage records.", response.Message);
            Assert.Empty(db.Damages);
            Assert.Empty(db.InventoryActions);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task GetDamages_Returns_Created_Damage_With_Actions()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<AuthUserSeeder>();
        var actionService = scope.ServiceProvider.GetRequiredService<InventoryActionService>();

        try
        {
            seeder.SeedAdmin(scope);
            var batch = await SeedBatchAsync(db, quantity: 10);

            await actionService.CreateDamageAsync(new AddDamageDto
            {
                InventoryActions = new List<AddInventoryActionDto>
                {
                    new()
                    {
                        InventoryBatchId = batch.Id,
                        Quantity = 1,
                        Notes = "Expired item"
                    }
                }
            });

            var response = await actionService.GetDamagesAsync(new DamageFilters
            {
                IsIncludeInventoryBatch = true,
                InventoryBatchId = batch.Id
            });

            Assert.Equal(200, response.Code);
            Assert.NotNull(response.Data);
            var damage = Assert.Single(response.Data!.Items);
            var action = Assert.Single(damage.Actions);

            Assert.Equal(batch.Id, action.InventoryBatchId);
            Assert.Equal("Expired item", action.Notes);
            Assert.NotNull(action.InventoryBatch);
            Assert.Equal(batch.BatchCode, action.InventoryBatch.BatchCode);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    private static async Task<InventoryBatch> SeedBatchAsync(AppDbContext db, decimal quantity)
    {
        var category = new Category { Name = $"cat_damage_{Guid.NewGuid():N}" };
        var unit = new Unit { Name = $"unit_damage_{Guid.NewGuid():N}", Symbol = "pc" };

        db.Categories.Add(category);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = $"damage_product_{Guid.NewGuid():N}",
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
            BatchCode = $"damage-batch-{Guid.NewGuid():N}",
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
