using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class ProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<ApiResponse<IList<ProductDto>>> GetAllProductsAsync(ProductFilters filters)
    {
        try
        {
            var response = await _productRepo.GetAllProductsAsync(filters);
            var data = response.Select(p => new ProductDto(p)
            ).ToList();
           
            return ApiResponse<IList<ProductDto>>.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<IList<ProductDto>>.Fail(ex.Message);
        }

    }

    public async Task<ApiResponse<string>> Add(UpdateProductDto product)
    {
        try
        {
            if(String.IsNullOrWhiteSpace(product.Name))
            {
                return ApiResponse<string>.Fail("Product Must Have a Name");
            }

            if(product.CategoryId == null)
            {
                return ApiResponse<string>.Fail("Product Must Have a Category");
            }

            if(product.UnitId == null)
            {
                return ApiResponse<string>.Fail("Product Must Have a Unit");
            }

            var newProduct = new Product
            {
                Name = product.Name,
                CategoryId = (int)product.CategoryId,
                UnitId = (int)product.UnitId,
                InternalCode = product.InternalCode,
                IsActive = product.IsActive ?? true,
                IsSellable = product.IsSellable ?? true,
                IsPurchasable = product.IsPurchasable ?? true,
                DoesExpire = product.DoesExpire ?? false
            };
            await _productRepo.AddProductAsync(newProduct);

            return ApiResponse<string>.Success("Product has been added");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<ProductDto>> Update(int id, UpdateProductDto dto)
    {
        try
        {
            var existingProduct = await _productRepo.GetProductByIdAsync(id);
            
            if(existingProduct == null)
            {
                return ApiResponse<ProductDto>.Fail("Product does not exist", 404);
            }
            
            existingProduct.Name = dto.Name ?? existingProduct.Name;
            existingProduct.CategoryId = dto.CategoryId ?? existingProduct.CategoryId;
            existingProduct.UnitId = dto.UnitId ?? existingProduct.UnitId;
            existingProduct.InternalCode = dto.InternalCode ?? existingProduct.InternalCode;
            existingProduct.IsActive = dto.IsActive ?? existingProduct.IsActive;
            existingProduct.IsSellable = dto.IsSellable ?? existingProduct.IsSellable;
            existingProduct.IsPurchasable = dto.IsPurchasable ?? existingProduct.IsPurchasable;
            existingProduct.DoesExpire = dto.DoesExpire ?? existingProduct.DoesExpire;

            await _productRepo.UpdateProductAsync(id, existingProduct);
            return ApiResponse<ProductDto>.Success(new ProductDto(existingProduct), "ProductUpdated");
        }
        catch (Exception e)
        {
            return ApiResponse<ProductDto>.Fail($"Some error occurred: {e.Message}", 400);
        }
    }

    public async Task<ApiResponse<ProductDto>> Delete(int id)
    {
        try
        {
            var existingProduct = await _productRepo.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                return ApiResponse<ProductDto>.Fail("Product does not exist", 404);
            }

            await _productRepo.DeleteProductAsync(id);
            return ApiResponse<ProductDto>.Success(new ProductDto(existingProduct), "ProductDeleted");
        }
        catch (Exception e)
        {
            return ApiResponse<ProductDto>.Fail($"An Error Occured {e.Message}", 400);
        }
    }
}
