using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Data_Layer.Repositories;

public class InventoryActionRepo : IInventoryActionRepository
{
    private readonly AppDbContext _context;
    private readonly IInventoryBatchRepository _batchRepo;

    public InventoryActionRepo(AppDbContext context, IInventoryBatchRepository batchRepo)
    {
        _context = context;
        _batchRepo = batchRepo;
    }

    // Inventory Actions
    public async Task<IList<InventoryAction>> GetInventoryActionsAsync(InventoryActionFilters filters)
    {
        var query = _context.InventoryActions.AsQueryable();

        if (filters.referenceId > 0)
        {
            query = query.Where(a => a.ReferenceId == filters.referenceId);
        }

        if (filters.batchId > 0)
        {
            query = query.Where(a => a.InventoryBatchId == filters.batchId);
        }

        if (filters.includeBatch)
        {
            query = query.Include(a => a.InventoryBatch).
                          ThenInclude(a => a.Product).
                          ThenInclude(p => p.Unit).
                          Include(a => a.InventoryBatch).
                          ThenInclude(a => a.Product).
                          ThenInclude(p => p.Category);

            if(filters.productId > 0)
            {
                query = query.Where(a => a.InventoryBatch.Product.Id == filters.productId);
            }
        }

        return await query.ToListAsync();
    }


    //Sales
    public async Task CreateSaleAsync(Sale newSale, IList<InventoryAction> inventoryActions)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {

            await validateInventoryActions(inventoryActions);

            _context.Sales.Add(newSale);

            await _context.SaveChangesAsync();

            foreach (var action in inventoryActions)
            {
                action.ReferenceId = newSale.Id;
                action.ReferenceType = InventoryReferenceType.Sale;
                action.ActionType = InventoryActionType.Sale;
            }

            _context.InventoryActions.AddRange(inventoryActions);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        catch(InvalidOperationException)
        {
            await transaction.RollbackAsync();
            throw;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PaginatedResult<Sale>> GetAllSaleAsync(SaleFilters filters)
    {
        var query = _context.Sales.AsQueryable();

        if(filters.Id > 0 )
        {
            query = query.Where(s => s.Id == filters.Id);
        }

        if(filters.CustomerId > 0)
        {
            query = query.Where(s => s.CustomerId == filters.CustomerId);
        }

        if(filters.IsIncludeUser == true)
        {
            query = query.Include(s => s.CreatedByNavigation);
        }

        if(filters.IsIncludeCustomer == true)
        {
            query = query.Include(s => s.Customer);
        }

        var totalItems = await query.CountAsync();

        var results = await query.Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();

        return new PaginatedResult<Sale>
        {
            Items = results,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }

    // Damages
    public async Task CreateDamageAsync(Damage newDamage, IList<InventoryAction> inventoryActions)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            await validateInventoryActions(inventoryActions);

            _context.Damages.Add(newDamage);

            await _context.SaveChangesAsync();

            foreach (var action in inventoryActions)
            {
                if(string.IsNullOrEmpty(action.Notes))
                {
                    throw new InvalidDataException("Notes are required for damage actions.");
                }

                action.ReferenceId = newDamage.Id;
                action.ReferenceType = InventoryReferenceType.Damage;
                action.ActionType = InventoryActionType.Damage;
            }

            _context.InventoryActions.AddRange(inventoryActions);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }

        catch (InvalidOperationException)
        {
            await transaction.RollbackAsync();
            throw;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
 
    public async Task<PaginatedResult<Damage>> GetAllDamageAsync(DamageFilters filters)
    {
        var query = _context.Damages.AsQueryable();

        if (filters.Id > 0)
        {
            query = query.Where(d => d.Id == filters.Id);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PaginatedResult<Damage>
        {
            Items = items,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }

    //helpers
    private async Task<InventoryBatchStock> GetBatchStockAsync(int batchId)
    {
        if (batchId <= 0)
        {
            return new InventoryBatchStock();
        }

        var batch = await _batchRepo.GetInventoryBatchByIdAsync(batchId);

        if (batch == null)
        {
            return new InventoryBatchStock();
        }

        var actions = await this.GetInventoryActionsAsync(
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

        return new InventoryBatchStock
        {
            BatchId = batchId,
            Quantity = batch.Quantity,
            Sold = totalSold,
            Damaged = damages,
            Returned = returns
        };
    }

    private async Task validateInventoryActions(IList<InventoryAction> inventoryActions)
    {
        foreach (var action in inventoryActions)
        {
            var batchStock = await this.GetBatchStockAsync(action.InventoryBatchId);
            if (batchStock == null || batchStock.BatchId <= 0)
            {
                throw new InvalidOperationException($"Batch with ID {action.InventoryBatchId} not found.");
            }
            var availableStock = batchStock.Quantity - batchStock.Sold - batchStock.Damaged + batchStock.Returned;
            if (action.Quantity > availableStock)
            {
                throw new InvalidOperationException($"Insufficient stock for Batch ID {action.InventoryBatchId}. Available: {availableStock}, Requested: {action.Quantity}");
            }
        }
    }

}