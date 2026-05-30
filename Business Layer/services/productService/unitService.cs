using Data_Layer.Interfaces;
using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.filters;
using Data_Layer.Entities;
using Data_Layer.commons;

namespace Business_Layer.services;

public class UnitService
{
    private readonly IProductRepository _productService;

    public UnitService(IProductRepository productService)
    {
        _productService = productService;
    }

    public async Task<ApiResponse<PaginatedResult<UnitDto>>> GetAll(UnitFilters filters)
    {
        try
        {
            var response = await _productService.GetAllUnitsAsync(filters);
            var units = response.Items;

            var data = new List<UnitDto>();

            foreach(var i in units)
            {
                data.Add(new UnitDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Symbol = i.Symbol,
                    CreatedAt = i.CreatedAt
                });
            }

            var paginatedDto = new PaginatedResult<UnitDto>
            {
                Items = data,
                Page = response.Page,
                PageSize = response.PageSize,
                TotalItems = response.TotalItems
            };

            return ApiResponse<PaginatedResult<UnitDto>>.Success(paginatedDto, "Data Returned Successfully", 200);
        }
        catch(Exception e)
        {
            return ApiResponse<PaginatedResult<UnitDto>>.Fail($"Some Error Occured {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<string>> Add(UpdateUnitDto unit)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(unit.Name))
            {
                return ApiResponse<string>.Fail("Unit name cannot be empty");
            }

            var data = new Unit {
                Name = unit.Name,
                Symbol = unit.Symbol ?? "",
                CreatedAt = DateTime.Now
            };

            await _productService.AddUnitAsync(data);

            return ApiResponse<String>.Success("", "Unit Has Been Added", 200);
        }
        catch(Exception e)
        {
            return ApiResponse<String>.Fail($"Some Error Occured {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<UnitDto>> Update(int id, UpdateUnitDto dto)
    {
        try
        {
            var existingUnit = await _productService.GetUnitByIdAsync(id);

            if (existingUnit == null)
            {
                return ApiResponse<UnitDto>.Fail("Unit does not exist", 404);
            }

            existingUnit.Name =
                dto.Name?.Trim() ?? existingUnit.Name;

            existingUnit.Symbol =
                dto.Symbol?.Trim() ?? existingUnit.Symbol;

            await _productService.UpdateUnitAsync(id, existingUnit);

            var response = new UnitDto
            {
                Id = existingUnit.Id,
                Name = existingUnit.Name,
                Symbol = existingUnit.Symbol,
                CreatedAt = existingUnit.CreatedAt
            };

            return ApiResponse<UnitDto>.Success(response, "Unit updated successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<UnitDto>.Fail($"Some error occurred: {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<UnitDto>> Delete(int id)
    {
        try
        {
            var unit = await _productService.GetUnitByIdAsync(id);
           
            if(unit != null)
            {
                await _productService.DeleteUnitAsync(id);
                return ApiResponse<UnitDto>.Success(new UnitDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    Symbol = unit.Symbol,
                    CreatedAt = unit.CreatedAt
                }, "Unit deleted successfully", 200);
            }
            else
            {
                return ApiResponse<UnitDto>.Fail("Unit Not Found", 404);
            }
        }
        catch(Exception e)
        {
            return ApiResponse<UnitDto>.Fail($"An Error Occured {e.Message}", 400);
        }
    }
}
