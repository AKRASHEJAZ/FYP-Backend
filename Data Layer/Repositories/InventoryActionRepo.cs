using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Repositories;

public class InventoryActionRepo : IInventoryActionRepository
{
    private readonly AppDbContext _context;

    public InventoryActionRepo(AppDbContext context)
    {
        _context = context;
    }
    public async Task CreateSaleAsync(Sale newSale, IList<InventoryAction> inventoryActions)
    {
        using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Sales.Add(newSale);

            await _context.SaveChangesAsync();

            foreach (var action in inventoryActions)
            {
                action.ReferenceId = newSale.Id;
                action.ReferenceType = InventoryReferenceType.Sale;
            }

            _context.InventoryActions.AddRange(inventoryActions);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
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

    public async Task<IList<InventoryAction>> GetInventoryActionsAsync(int referenceId, bool includeBatch)
    {
        var query = _context.InventoryActions.AsQueryable();

        query = query.Where(a => a.ReferenceId == referenceId);

        if(includeBatch)
        {
            query = query.Include(a => a.InventoryBatch).
                          ThenInclude(a => a.Product).
                          ThenInclude(p => p.Unit).
                          Include(a => a.InventoryBatch).
                          ThenInclude(a => a.Product).
                          ThenInclude(p => p.Category);
        }

        return await query.ToListAsync();
    }
}
