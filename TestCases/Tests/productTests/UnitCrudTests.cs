using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.filters;
using Microsoft.Extensions.DependencyInjection;
using TestCases.Common;

namespace TestCases.Tests;

public class UnitCrudTests
{
    [Fact]
    public async Task Unit_CRUD_Flow()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var unitService = scope.ServiceProvider.GetRequiredService<UnitService>();

        try
        {
            // Create
            var createResponse = await unitService.Add(new UpdateUnitDto
            {
                Name = "unit_crud_1",
                Symbol = "kg"
            });

            Assert.Equal(200, createResponse.Code);

            // Read
            var getResponse = await unitService.GetAll(new UnitFilters
            {
                Name = new List<string> { "unit_crud_1" }
            });

            Assert.Equal(200, getResponse.Code);
            Assert.NotNull(getResponse.Data);
            Assert.NotEmpty(getResponse.Data!);

            var created = getResponse.Data!.FirstOrDefault();
            Assert.NotNull(created);
            Assert.True(created!.Id > 0);

            // Update
            var updateResponse = await unitService.Update(created.Id, new UpdateUnitDto
            {
                Name = "unit_crud_1_updated",
                Symbol = "g"
            });

            Assert.Equal(200, updateResponse.Code);
            Assert.NotNull(updateResponse.Data);
            Assert.Equal("unit_crud_1_updated", updateResponse.Data!.Name);
            Assert.Equal("g", updateResponse.Data!.Symbol);

            // Delete
            var deleteResponse = await unitService.Delete(created.Id);

            Assert.Equal(200, deleteResponse.Code);

            var afterDelete = await unitService.GetAll(new UnitFilters
            {
                Id = created.Id
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

    [Fact]
    public async Task Unit_Add_With_Empty_Name_Returns_Validation_Error()
    {
        var provider = TestServices.Create();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var unitService = scope.ServiceProvider.GetRequiredService<UnitService>();

        try
        {
            var emptyName = await unitService.Add(new UpdateUnitDto { Name = "", Symbol = "kg" });
            Assert.Equal(400, emptyName.Code);
            Assert.Equal("Unit name cannot be empty", emptyName.Message);

            var whitespaceName = await unitService.Add(new UpdateUnitDto { Name = "   ", Symbol = "kg" });
            Assert.Equal(400, whitespaceName.Code);
            Assert.Equal("Unit name cannot be empty", whitespaceName.Message);
        }
        finally
        {
            db.Database.EnsureDeleted();
        }
    }
}

