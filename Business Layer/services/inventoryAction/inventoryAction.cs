using Business_Layer.Common;
using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;

namespace Business_Layer.services;

public class InventoryActionService
{
    private readonly IInventoryActionRepository _repo;
    private readonly IInventoryBatchRepository _batchRepo;
    private readonly UserService _user;

    public InventoryActionService(IInventoryActionRepository repo, IInventoryBatchRepository inventoryBatch, UserService user)
    {
        _repo = repo;
        _batchRepo = inventoryBatch;
        _user = user;
    }

    public async Task<ApiResponse<string>> CreateSaleAsync(AddSaleDto dto)
    {
        try
        {
            var (flowControl, value) = await ValidateSaleAsync(dto);
            
            if (!flowControl)
            {
                return value!;
            }

            var authResult = _user.GetAuthUser();
            var authUser = authResult.Data;

            if (authResult.Code != 200 || authResult.Data == null)
                return ApiResponse<string>.Fail("Unauthorized", 401);

            var newSale = new Sale 
            {
                CustomerId = dto.CustomerId,
                SaleDate = DateTime.Now,
                CreatedBy = (int)authUser!.Id!,
            };

            var actions = dto.InventoryActions.Select(x => new InventoryAction
            {
                InventoryBatchId = x.InventoryBatchId,
                Quantity = x.Quantity,
                CreatedBy = (int)authUser!.Id!,
            }).ToList();

            await _repo.CreateSaleAsync(newSale, actions);

            return ApiResponse<string>.Success("Sale created successfully");
        }
        catch (Exception ex)
        {
             return ApiResponse<string>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<PaginatedResult<SaleDto>>> GetSalesAsync(SaleFilters filters)
    {
        try
        {
            var result = await _repo.GetAllSaleAsync(filters);

            var sales = result.Items;

            if(sales == null || sales.Count <= 0 )
            {
                return ApiResponse<PaginatedResult<SaleDto>>.Fail("No sale found");
            }

            var returnData = new List<SaleDto>();

            foreach (var sale in sales)
            {
                var actions = await _repo.GetInventoryActionsAsync(sale.Id, filters.IsIncludeInventoryBatch ?? false);
                returnData.Add(new SaleDto(sale, actions));
            }

            var paginatedResult = new PaginatedResult<SaleDto>
            { 
                Items = returnData,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems
            };

            return ApiResponse<PaginatedResult<SaleDto>>.Success(paginatedResult);
        }
        catch (Exception e)
        {
            return ApiResponse<PaginatedResult<SaleDto>>.Fail($"Some Error Occured: {e.Message}");
        }
    }
    // Helpers
    private async Task<(bool flowControl, ApiResponse<string>? value)> ValidateSaleAsync(AddSaleDto dto)
    {
        if (dto.InventoryActions.Count <= 0)
        {
            return (flowControl: false, value: ApiResponse<string>.Fail("Please Specify any action"));
        }

        if (dto.InventoryActions.Any(x => x.Quantity <= 0))
        {
            return (flowControl: false, value: ApiResponse<string>.Fail("Quantity must be greater than zero."));
        }

        // Avoid running multiple EF operations concurrently on the same DbContext.
        foreach (var action in dto.InventoryActions)
        {
            var batch = await _batchRepo.GetInventoryBatchByIdAsync(action.InventoryBatchId);
            if (batch == null)
            {
                return (flowControl: false, value: ApiResponse<string>.Fail("Invalid Batch Id Entered"));
            }
        }

        return (flowControl: true, value: null);
    }
}
