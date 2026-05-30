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
    private readonly InventoryBatchService _batchService;

    public InventoryActionService(IInventoryActionRepository repo, IInventoryBatchRepository inventoryBatch, UserService user, InventoryBatchService batchService)
    {
        _repo = repo;
        _batchRepo = inventoryBatch;
        _user = user;
        _batchService = batchService;
    }

    public async Task<ApiResponse<string>> CreateSaleAsync(AddSaleDto dto)
    {
        try
        {
            var (isValid, errorResult) = await ValidateSaleAsync(dto);

            if (!isValid)
            {
                return errorResult!;
            }

            (isValid, errorResult) = await ValidateActionsAsync(dto.InventoryActions, false);

            if (!isValid)
            {
                return errorResult!;
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
                var actions = await _repo.GetInventoryActionsAsync(new InventoryActionFilters{ 
                    referenceId = sale.Id,
                    includeBatch = filters.IsIncludeInventoryBatch ?? false
                    });
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

    //Damages
    public async Task<ApiResponse<string>> CreateDamageAsync(AddDamageDto dto)
    {
        try
        {
            var (isValid, errorResult) = await ValidateActionsAsync(dto.InventoryActions, true);

            if (!isValid)
            {
                return errorResult!;
            }

            var authResult = _user.GetAuthUser();
            var authUser = authResult.Data;
            if (authResult.Code != 200 || authResult.Data == null)
                return ApiResponse<string>.Fail("Unauthorized", 401);
            var newDamage = new Damage 
            {   DamageDate = DateTime.Now,
                CreatedBy = (int)authUser!.Id!,
            };
            var actions = dto.InventoryActions.Select(x => new InventoryAction
            {
                InventoryBatchId = x.InventoryBatchId,
                Quantity = x.Quantity,
                CreatedBy = (int)authUser!.Id!,
                Notes = x.Notes,
            }).ToList();
            await _repo.CreateDamageAsync(newDamage, actions);
            return ApiResponse<string>.Success("Damage record created successfully");
        }
        catch (Exception ex)
        {
             return ApiResponse<string>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<PaginatedResult<DamageDto>>> GetDamagesAsync(DamageFilters filters)
    {
        try
        {
            var result = await _repo.GetAllDamageAsync(filters);
            var damages = result.Items;
            
            if(damages == null || damages.Count <= 0 )
            {
                return ApiResponse<PaginatedResult<DamageDto>>.Fail("No damage record found");
            }
            
            var returnData = new List<DamageDto>();
            
            foreach (var damage in damages)
            {
                var actions = await _repo.GetInventoryActionsAsync(new InventoryActionFilters{ 
                    referenceId = damage.Id,
                    includeBatch = filters.IsIncludeInventoryBatch ?? false,
                    batchId = filters.InventoryBatchId,
                    productId = filters.productId,
                });
                returnData.Add(new DamageDto(damage, actions));
            }
            var paginatedResult = new PaginatedResult<DamageDto>
            { 
                Items = returnData,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalItems = result.TotalItems
            };
            return ApiResponse<PaginatedResult<DamageDto>>.Success(paginatedResult);
        }
        catch (Exception e)
        {
            return ApiResponse<PaginatedResult<DamageDto>>.Fail($"Some Error Occured: {e.Message}");
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

        return (flowControl: true, value: null);
    }

    private async Task<(bool flowControl, ApiResponse<string>? value)> ValidateActionsAsync(IList<AddInventoryActionDto> actions, bool checkNotes)
    {
        foreach (var action in actions)
        {
            var batch = await _batchRepo.GetInventoryBatchByIdAsync(action.InventoryBatchId);

            if (batch == null)
            {
                return (flowControl: false, value: ApiResponse<string>.Fail("Invalid Batch Id Entered"));
            }

            if (action.Quantity <= 0)
            {
                return (
                    flowControl: false,
                    value: ApiResponse<string>.Fail("Quantity must be greater than zero")
                );
            }

            var stocks = _batchService.GetBatchStockAsync(batch.Id).Result.Data;

            if (stocks == null)
            {
                return (
                    flowControl: false,
                    value: ApiResponse<string>.Fail($"Unable to retrieve stock information for batch ID {batch.Id}")
                );
            }

            if (stocks.AvailableStock < action.Quantity)
            {
                return (
                    flowControl: false,
                    value: ApiResponse<string>.Fail(
                        $"Insufficient stock for batch {batch.BatchCode} - {batch.Product?.Name ?? ""}. Available stock: {stocks.AvailableStock}"
                    )
                );
            }

            if(checkNotes && string.IsNullOrWhiteSpace(action.Notes) ) 
            {
                return (
                    flowControl: false,
                    value: ApiResponse<string>.Fail("Notes are required for damage records.")
                );
            }
        }
        return (flowControl: true, value: null);
    }

}
