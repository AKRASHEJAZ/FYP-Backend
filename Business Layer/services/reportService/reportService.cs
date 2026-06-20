
using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class ReportService
{

    private readonly IReportRepository _repo;
    private readonly InventoryBatchService _batchService;

    public ReportService(IReportRepository repo, InventoryBatchService batchService)
    {
        _repo = repo;
        _batchService = batchService;
    }

    public async Task<ApiResponse<CountData>> GetCountData()
    {
        try
        {
            var data = await _repo.GetCountData();
            return ApiResponse<CountData>.Success(data);
        }
        catch(Exception e)
        {
            return ApiResponse<CountData>.Fail($"Some Error Occured {e.Message}");
        }
    }

    public async Task<ApiResponse<PaginatedResult<InventoryBatchDto>>> GetExpiredProduct(ExpiryReportFilters filters)
    {
        try
        {
            var data = await _repo.GetExpiredBatches(filters);

            var batches = data.Items;

            if(batches.Count == 0)
            {
                return ApiResponse<PaginatedResult<InventoryBatchDto>>.Fail("No expired batches found", 404);
            }

            var batchDtos = new List<InventoryBatchDto>();

            foreach(var batch in batches)
            {
                var stockData = await _batchService.GetBatchStockAsync(batch.Id);
                
                // Skip SoldOut Products
                if(stockData.Data?.AvailableStock == 0)
                {
                    continue;
                }

                var batchDto = new InventoryBatchDto
                {
                    Id = batch.Id,
                    ExpiryDate = batch.ExpiryDate,
                    Stocks = stockData.Data,
                    ProductId = batch.ProductId,
                    Product = new ProductDto
                    {
                        Id = batch.Product.Id,
                        Name = batch.Product.Name,
                        CategoryId = batch.Product.CategoryId,
                    }
                };
                batchDtos.Add(batchDto);
            }

            var paginatedResult = new PaginatedResult<InventoryBatchDto>
            {
                Items = batchDtos,
                Page = data.Page,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems
            };

            return ApiResponse<PaginatedResult<InventoryBatchDto>>.Success(paginatedResult);
        }
        catch(Exception e)
        {
            return ApiResponse<PaginatedResult<InventoryBatchDto>>.Fail($"Some Error Occured {e.Message}");
        }
    }
}
