using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class InventoryBatchService
{
    private readonly IInventoryBatchRepository _repository;
    private readonly IProductRepository _productRepository;

    public InventoryBatchService(IInventoryBatchRepository repository, IProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponse<List<InventoryBatchDto>>> GetAllInventoryBatchesAsync(InventoryBatchFilters filters)
    {
        try
        {
            var batches = await _repository.GetAllInventoryBatchesAsync(filters);
            var data = batches.Select(b => new InventoryBatchDto(b)).ToList();
            return ApiResponse<List<InventoryBatchDto>>.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<InventoryBatchDto>>.Fail($"Error fetching inventory batches: {ex.Message}");
        }
    }

    public async Task<ApiResponse<InventoryBatchDto>> GetInventoryBatchByIdAsync(int id)
    {
        try
        {
            var batch = await _repository.GetInventoryBatchByIdAsync(id);
            if (batch == null)
                return ApiResponse<InventoryBatchDto>.Fail("Inventory batch not found.");
            var data = new InventoryBatchDto(batch);
            return ApiResponse<InventoryBatchDto>.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryBatchDto>.Fail($"Error fetching inventory batch: {ex.Message}");
        }
    }

    public async Task<ApiResponse<InventoryBatchDto>> AddInventoryBatchAsync(AddInventoryBatchDto newBatch)
    {
        try
        {
            var product = await _productRepository.GetProductByIdAsync(newBatch.ProductId);
            
            (bool flowControl, ApiResponse<InventoryBatchDto> value) = ValidateStockEntry(newBatch, product);
            if (!flowControl)
            {
                return value;
            }

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var random = Random.Shared.Next(10, 99);

            var batchCode = $"{product!.Name?.ToLower() ?? "unknown"}-{timestamp}-{random}";

            var batch = new InventoryBatch
            {
                ProductId = newBatch.ProductId,
                BatchCode = batchCode,

                PurchasePrice = newBatch.PurchasePrice,
                SellingPrice = newBatch.SellingPrice,

                Quantity = newBatch.PurchasedQuantity,

                Mfgdate = newBatch.MFGDate,
                ExpiryDate = newBatch.ExpiryDate,

                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddInventoryBatchAsync(batch);

            var data = new InventoryBatchDto(batch);

            return ApiResponse<InventoryBatchDto>.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryBatchDto>
                .Fail($"Error adding inventory batch: {ex.Message}");
        }
    }

    private static (bool flowControl, ApiResponse<InventoryBatchDto> value) ValidateStockEntry(AddInventoryBatchDto newBatch, Product? product)
    {
        if (product == null)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Product not found for the given ProductId."));

        if (!product.IsActive)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Cannot add batch for an inactive product."));

        if (!product.IsPurchasable)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Product is not purchasable."));

        if (product.DoesExpire)
        {
            if (newBatch.ExpiryDate == null)
                return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                    .Fail("Expiry date is required for products that expire."));

            if (newBatch.ExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                    .Fail("Expiry date must be in the future."));

            if (newBatch.MFGDate != null &&
                newBatch.ExpiryDate <= newBatch.MFGDate)
                return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                    .Fail("Expiry date must be after manufacturing date."));
        }

        if (newBatch.PurchasePrice <= 0)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Purchase price must be greater than zero."));

        if (newBatch.SellingPrice <= 0)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Selling price must be greater than zero."));

        if (newBatch.SellingPrice < newBatch.PurchasePrice)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Selling price cannot be less than purchase price."));

        if (newBatch.PurchasedQuantity <= 0)
            return (flowControl: false, value: ApiResponse<InventoryBatchDto>
                .Fail("Purchased quantity must be greater than zero."));
        return (flowControl: true, value: ApiResponse<InventoryBatchDto>.Success(new InventoryBatchDto()));
    }
}
