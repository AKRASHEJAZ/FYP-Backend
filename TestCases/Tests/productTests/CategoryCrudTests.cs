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
            Assert.NotEmpty(getResponse.Data!.Items);

            var created = getResponse.Data!.Items.FirstOrDefault();
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

            Assert.Equal(400, afterDelete.Code);
            Assert.Equal("No Categories Found", afterDelete.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }

    [Fact]
    public async Task Category_Add_With_Empty_Name_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();

        try
        {
            var emptyName = await categoryService.AddCategoryAsync(new UpdateCategoryDto { Name = "" });
            Assert.Equal(400, emptyName.Code);
            Assert.Equal("Category name cannot be empty", emptyName.Message);

            var whitespaceName = await categoryService.AddCategoryAsync(new UpdateCategoryDto { Name = "   " });
            Assert.Equal(400, whitespaceName.Code);
            Assert.Equal("Category name cannot be empty", whitespaceName.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }
}

