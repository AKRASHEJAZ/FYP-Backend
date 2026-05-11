using Data_Layer.Interfaces;
using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.filters;
using Data_Layer.Entities;

namespace Business_Layer.services;

public class UnitService
{
    private readonly IProductRepository _productService;

    public UnitService(IProductRepository productService)
    {
        _productService = productService;
    }

    public async Task<ApiResponse<IList<UnitDto>>> GetAll(UnitFilters filters)
    {
        try
        {
            var response = await _productService.GetAllUnitsAsync(filters);

            //Model The Data Here
            var data = new List<UnitDto>();

            foreach(var i in response)
            {
                data.Add(new UnitDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Symbol = i.Symbol,
                    CreatedAt = i.CreatedAt
                });
            }
            return ApiResponse<IList<UnitDto>>.Success(data, "Data Returned Successfully", 200);
        }
        catch(Exception e)
        {
            return ApiResponse<IList<UnitDto>>.Fail($"Some Error Occured {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<string>> Add(UpdateUnitDto unit)
    {
        try
        {
            var data = new Unit {
                Name = unit.Name,
                Symbol = unit.Symbol,
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
