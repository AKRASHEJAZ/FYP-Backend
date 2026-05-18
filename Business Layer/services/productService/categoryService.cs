using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class CategoryService
{
    private readonly IProductRepository _productService;

    public CategoryService(IProductRepository productService)
    {
        _productService = productService;
    }

    public async Task<ApiResponse<PaginatedResult<CategoryDto>>> GetAllCategoriesAsync(CategoryFilters filters)
    {
        try
        {
            var response = await _productService.GetAllCategoriesAsync(filters);
            var categories = response.Items;

            if(categories == null || categories.Count <= 0)
            {
                return ApiResponse<PaginatedResult<CategoryDto>>.Fail("No Categories Found");
            }

            var data = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt
            }).ToList();

            var paginatedDtoResult = new PaginatedResult<CategoryDto>
            {
                Items = data,
                Page = response.Page,
                PageSize = response.PageSize,
                TotalItems = response.TotalItems
            };

            return ApiResponse<PaginatedResult<CategoryDto>>.Success(paginatedDtoResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<CategoryDto>>.Fail(ex.Message);
        }

    }

    public async Task<ApiResponse<string>> AddCategoryAsync(UpdateCategoryDto category)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(category.Name))
            {
                return ApiResponse<string>.Fail("Category name cannot be empty");
            }
            var newCategory = new Category
            {
                Name = category.Name.Trim()
            };
            await _productService.AddCategoryAsync(newCategory);
            return ApiResponse<string>.Success("Category added successfully"); 
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> Update(int id, UpdateCategoryDto dto)
    {
        try
        {
            var existingCategory = await _productService.GetCategoryByIdAsync(id);

            if (existingCategory == null)
            {
                return ApiResponse<CategoryDto>.Fail("Category does not exist", 404);
            }

            existingCategory.Name =
                dto.Name?.Trim() ?? existingCategory.Name;

            await _productService.UpdateCategoryAsync(id, existingCategory);

            var response = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                CreatedAt = existingCategory.CreatedAt
            };

            return ApiResponse<CategoryDto>.Success(response, "Category updated successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<CategoryDto>.Fail($"Some error occurred: {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<CategoryDto>> Delete(int id)
    {
        try
        {
            var category = await _productService.GetCategoryByIdAsync(id);

            if (category != null)
            {
                await _productService.DeleteCategoryAsync(id);
                return ApiResponse<CategoryDto>.Success(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    CreatedAt = category.CreatedAt
                }, "Category deleted successfully", 200);
            }
            else
            {
                return ApiResponse<CategoryDto>.Fail("Category Not Found", 404);
            }
        }
        catch (Exception e)
        {
            return ApiResponse<CategoryDto>.Fail($"An Error Occured {e.Message}", 400);
        }
    }
}
