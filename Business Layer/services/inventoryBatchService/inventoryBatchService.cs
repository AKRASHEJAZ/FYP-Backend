using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class InventoryBatchService
{
    private readonly IInventoryBatchRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryActionRepository _inventoryActionRepository;

    public InventoryBatchService(IInventoryBatchRepository repository, IProductRepository productRepository, IInventoryActionRepository inventoryActionRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _inventoryActionRepository = inventoryActionRepository;
    }

    public async Task<ApiResponse<PaginatedResult<InventoryBatchDto>>> GetAllInventoryBatchesAsync(InventoryBatchFilters filters)
    {
        try
        {
            var result = await _repository.GetAllInventoryBatchesAsync(filters);
            var batches = result.Items;

            if (batches == null || batches.Count == 0)
            {
                return ApiResponse<PaginatedResult<InventoryBatchDto>>.Fail("No Batches Found");
            }

            var data = batches.Select(b => new InventoryBatchDto(b)).ToList();

            foreach (var d in data)
            {
                var stockResponse = await GetBatchStockAsync(d.Id);
                if (stockResponse.Code == 200)
                {
                    d.Stocks = stockResponse.Data;
                }
            }

            var paginatedResult = new PaginatedResult<InventoryBatchDto>
            {
                Items = data,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems
            };

            return ApiResponse<PaginatedResult<InventoryBatchDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<InventoryBatchDto>>.Fail($"Error fetching inventory batches: {ex.Message}");
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
            var stockResponse = await GetBatchStockAsync(id);
            if (stockResponse.Code == 200)
            {
                data.Stocks = stockResponse.Data;
            }
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

            data.Stocks = new InventoryBatchStock
            {
                BatchId = batch.Id,
                Quantity = batch.Quantity,
                Sold = 0,
                Damaged = 0,
                Returned = 0
            };

            return ApiResponse<InventoryBatchDto>.Success(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryBatchDto>
                .Fail($"Error adding inventory batch: {ex.Message}");
        }
    }

    public async Task<ApiResponse<InventoryBatchStock>> GetBatchStockAsync(int batchId)
    {
        if (batchId <= 0)
        {
            return ApiResponse<InventoryBatchStock>.Fail("Please Enter a Valid Id");
        }

        var batch = await _repository.GetInventoryBatchByIdAsync(batchId);

        if (batch == null)
        {
            return ApiResponse<InventoryBatchStock>.Fail("No Inventory Batch Found");
        }

        var actions = await _inventoryActionRepository.GetInventoryActionsAsync(
            new InventoryActionFilters
            {
                batchId = batchId
            });

        decimal totalSold = 0;
        decimal damages = 0;
        decimal returns = 0;

        foreach (var action in actions)
        {
            if (action.ActionType == InventoryActionType.Sale ||
                action.ReferenceType == InventoryReferenceType.Sale)
            {
                totalSold += action.Quantity;
            }

            if (action.ActionType == InventoryActionType.Damage ||
                action.ReferenceType == InventoryReferenceType.Damage)
            {
                damages += action.Quantity;
            }

            if (action.ActionType == InventoryActionType.Return ||
                action.ReferenceType == InventoryReferenceType.Return)
            {
                returns += action.Quantity;
            }
        }

        return ApiResponse<InventoryBatchStock>.Success(new InventoryBatchStock
        {
            BatchId = batchId,
            Quantity = batch.Quantity,
            Sold = totalSold,
            Damaged = damages,
            Returned = returns
        });
    }

    // Helpers
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
