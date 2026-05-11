using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;

namespace TestCases.Tests;

public class CategoryCrudTests
{
    [Fact]
    public async Task Category_CRUD_Flow()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();

        try
        {
            // Create
            var createResponse = await categoryService.AddCategoryAsync(new UpdateCategoryDto
            {
                Name = "cat_crud_1"
            });

            Assert.Equal(200, createResponse.Code);

            // Read
            var getResponse = await categoryService.GetAllCategoriesAsync(new CategoryFilters
            {
                Name = new List<string> { "cat_crud_1" }
            });

            Assert.Equal(200, getResponse.Code);
            Assert.NotNull(getResponse.Data);
            Assert.NotEmpty(getResponse.Data!);

            var created = getResponse.Data!.FirstOrDefault();
            Assert.NotNull(created);
            Assert.True(created!.Id > 0);

            // Update
            var updateResponse = await categoryService.Update(created.Id, new UpdateCategoryDto
            {
                Name = "cat_crud_1_updated"
            });

            Assert.Equal(200, updateResponse.Code);
            Assert.NotNull(updateResponse.Data);
            Assert.Equal("cat_crud_1_updated", updateResponse.Data!.Name);

            // Delete
            var deleteResponse = await categoryService.Delete(created.Id);

            Assert.Equal(200, deleteResponse.Code);

            var afterDelete = await categoryService.GetAllCategoriesAsync(new CategoryFilters
            {
                Name = new List<string> { "cat_crud_1_updated" }
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

