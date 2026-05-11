using Business_Layer.services;
using Business_Layer.DTOS;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;
namespace TestCases.Tests;

public class ProductCrudTests
{
    [Fact]
    public async Task Product_CRUD_Flow()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var productService = scope.ServiceProvider.GetRequiredService<ProductService>();

        try
        {
            // Seed: category + unit (required for product)
            var category = new Category { Name = "cat_product_crud" };
            var unit = new Unit { Name = "unit_product_crud", Symbol = "pc" };

            db.Categories.Add(category);
            db.Units.Add(unit);
            await db.SaveChangesAsync();

            // Create
            var createResponse = await productService.Add(new UpdateProductDto
            {
                Name = "product_crud_1",
                CategoryId = category.Id,
                UnitId = unit.Id,
                InternalCode = "IC-001",
                IsActive = true,
                IsSellable = true,
                IsPurchasable = true,
                DoesExpire = false
            });

            Assert.Equal(200, createResponse.Code);

            // Read (via service)
            var getResponse = await productService.GetAllProductsAsync(new ProductFilters
            {
                Name = new List<string> { "product_crud_1" }
            });

            Assert.Equal(200, getResponse.Code);
            Assert.NotNull(getResponse.Data);
            Assert.NotEmpty(getResponse.Data);

            var created = getResponse.Data!.FirstOrDefault();
            Assert.NotNull(created);
            Assert.True(created!.Id > 0);

            // Update
            var updateResponse = await productService.Update(created.Id, new UpdateProductDto
            {
                Name = "product_crud_1_updated",
                IsActive = false
            });

            Assert.Equal(200, updateResponse.Code);
            Assert.NotNull(updateResponse.Data);
            Assert.Equal("product_crud_1_updated", updateResponse.Data!.Name);
            Assert.False(updateResponse.Data!.IsActive);

            // Delete
            var deleteResponse = await productService.Delete(created.Id);

            Assert.Equal(200, deleteResponse.Code);

            var afterDelete = await productService.GetAllProductsAsync(new ProductFilters
            {
                Id = new List<int> { created.Id }
            });

            Assert.Equal(200, afterDelete.Code);
            Assert.NotNull(afterDelete.Data);
            Assert.Empty(afterDelete.Data!);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }
}
