
using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Data_Layer.Repositories;

public class InventoryBatchRepo : IInventoryBatchRepository
{
    private readonly AppDbContext _context;

    public InventoryBatchRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddInventoryBatchAsync(InventoryBatch inventoryBatch)
    {
        await _context.InventoryBatches.AddAsync(inventoryBatch);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResult<InventoryBatch>> GetAllInventoryBatchesAsync(InventoryBatchFilters filters)
    {
        var query = _context.InventoryBatches.AsNoTracking().AsQueryable();

        query = query.Include(c => c.Product).
            ThenInclude(p => p.Category).
            Include(c => c.Product).
            ThenInclude(p => p.Unit);

        if (filters.Id?.Count > 0)
        {
            query = query.Where(c => filters.Id.Contains(c.Id));
        }

        if (filters.ProductId?.Count > 0)
        {
            query = query.Where(c => filters.ProductId.Contains(c.ProductId));
        }

        if (filters.BatchCode?.Count > 0)
        {
            query = query.Where(c => filters.BatchCode.Contains(c.BatchCode ?? ""));
        }

        if (filters.ExpiryDate.HasValue)
        {
            query = query.Where(c => c.ExpiryDate == (DateOnly)filters.ExpiryDate.Value);
        }

        var totalItems = query.Count();

        var data = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();

        return new PaginatedResult<InventoryBatch>
        {
            Items = data,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }
    public async Task<InventoryBatch?> GetInventoryBatchByIdAsync(int id)
    {
        return await _context.InventoryBatches.FindAsync(id);
    }
}
