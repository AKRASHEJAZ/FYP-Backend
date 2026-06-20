using Business_Layer.DTOS;
using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.enums;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Repositories;

public class ReportRepo : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CountData> GetCountData()
    {
        // Getting Product Count
        var totalProducts = await _context.Products.CountAsync();
        var activeProduct = await _context.Products.Where(p => p.IsActive).CountAsync();

        // Getting User Info
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.Where(u => u.IsActive).CountAsync();

        // Getting Sales 
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var sales = await _context.InventoryActions
            .Include(ia => ia.InventoryBatch)
            .Where(ia =>
                ia.ReferenceType == InventoryReferenceType.Sale &&
                ia.ActionType == InventoryActionType.Sale &&
                _context.Sales.Any(s =>
                    s.Id == ia.ReferenceId &&
                    s.SaleDate >= today &&
                    s.SaleDate < tomorrow))
            .ToListAsync();

        var totalSale = sales.Sum(action =>
            action.Quantity * action.InventoryBatch.SellingPrice);


        var damages = await _context.InventoryActions
            .Include(ia => ia.InventoryBatch)
            .Where(ia =>
                ia.ReferenceType == InventoryReferenceType.Damage &&
                ia.ActionType == InventoryActionType.Damage &&
                _context.Damages.Any(s =>
                    s.Id == ia.ReferenceId &&
                    s.DamageDate >= today &&
                    s.DamageDate < tomorrow))
            .ToListAsync();

        var totalDamage = damages.Sum(action =>
            action.Quantity * action.InventoryBatch.SellingPrice);

        var returns = await _context.InventoryActions
            .Include(ia => ia.InventoryBatch)
            .Where(ia =>
                ia.ReferenceType == InventoryReferenceType.Return &&
                ia.ActionType == InventoryActionType.Return &&
                _context.Returns.Any(s =>
                    s.Id == ia.ReferenceId &&
                    s.ReturnDate >= today &&
                    s.ReturnDate < tomorrow))
            .ToListAsync();

        var totalReturn = returns.Sum(action =>
            action.Quantity * action.InventoryBatch.SellingPrice);

        return new CountData
        {
            TotalProducts = totalProducts,
            ActiveProduct = activeProduct,
            UserData = new UserCount 
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers
            },
            TotalSale = totalSale,
            TotalDamages = totalDamage,
            TotalReturns = totalReturn
        };
    }

    public async Task<PaginatedResult<InventoryBatch>> GetExpiredBatches(ExpiryReportFilters filters)
    {
        var dateOnly = DateOnly.FromDateTime(filters.Date);

        var query = _context.InventoryBatches
            .Include(ib => ib.Product)
            .Where(ib =>
                ib.Product.DoesExpire &&
                ib.ExpiryDate < dateOnly);

        if(filters.ProductIds.Count > 0)
        {
            query = query.Where(ib => filters.ProductIds.Contains(ib.Product.Id));
        }

        var totalCount = query.Count(); 

        var result = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PaginatedResult<InventoryBatch> 
        {
            Items = result,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalCount
        };
    }
}

