using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using System.Text.RegularExpressions;

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

            if (product == null)
                return ApiResponse<InventoryBatchDto>
                    .Fail("Product not found for the given ProductId.");

            var cleanName = Regex.Replace(product.Name.ToLower(), @"[^a-z0-9]", "");

            if (cleanName.Length > 6)
                cleanName = cleanName[..6];

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var random = Random.Shared.Next(10, 99);

            var batchCode = $"{cleanName}-{timestamp}-{random}";

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
}
