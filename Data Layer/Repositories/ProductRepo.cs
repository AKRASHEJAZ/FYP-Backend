using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Repositories;

public class ProductRepo : IProductRepository
{

    private readonly AppDbContext _context;

    public ProductRepo(AppDbContext context)
    {
        _context = context;
    }

    // Categories
    async Task<Category?> IProductRepository.GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }
    async Task<PaginatedResult<Category>> IProductRepository.GetAllCategoriesAsync(CategoryFilters filters)
    {
        IQueryable<Category> query = _context.Categories;

        if (filters.Id > 0)
        {
            query = query.Where(c => c.Id == filters.Id);
        }

        if (filters.Name?.Count > 0)
        {
            foreach (var name in filters.Name)
            {
                var temp = name;
                query = query.Where(c => c.Name.StartsWith(temp));
            }
        }

        var totalItems = query.Count();

        var categories =  await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();
       
        return new PaginatedResult<Category>
        {
            Items = categories,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }

    async Task IProductRepository.AddCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    async Task IProductRepository.UpdateCategoryAsync(int id, Category category)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory != null)
        {
            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();
        }
    }

    async Task IProductRepository.DeleteCategoryAsync(int id)
    {
        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory != null)
        {
            _context.Categories.Remove(existingCategory);
            await _context.SaveChangesAsync();
        }
    }

    // Units
    async Task<Unit?> IProductRepository.GetUnitByIdAsync(int id)
    {
        return await _context.Units.FirstOrDefaultAsync(u => u.Id == id);
    }
    async Task<PaginatedResult<Unit>> IProductRepository.GetAllUnitsAsync(UnitFilters filters)
    {
        IQueryable<Unit> query = _context.Units;

        if (filters.Id > 0)
        {
            query = query.Where(u => u.Id == filters.Id);
        }

        if (filters.Name?.Count > 0)
        {
            foreach (var name in filters.Name)
            {
                var temp = name;
                query = query.Where(c => c.Name.StartsWith(temp));
            }
        }

        if (filters.Symbol?.Count > 0)
        {
            query = query.Where(u => filters.Symbol.Contains(u.Symbol));
        }

        var totalItems = query.Count();

        var units = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();

        return new PaginatedResult<Unit>
        {
            Items = units,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }
    
    async Task IProductRepository.AddUnitAsync(Unit unit)
    {
        await _context.Units.AddAsync(unit);
        await _context.SaveChangesAsync();
    }

    async Task IProductRepository.UpdateUnitAsync(int id, Unit unit)
    {
        var existingUnit = await _context.Units.FindAsync(id);
        if (existingUnit != null)
        {
            if(unit.Name != null)
                existingUnit.Name = unit.Name;
            
            if(unit.Symbol != null)
            existingUnit.Symbol = unit.Symbol;
            
            await _context.SaveChangesAsync();
        }
    }

    async Task IProductRepository.DeleteUnitAsync(int id)
    {
        var existingUnit = await _context.Units.FindAsync(id);
        if (existingUnit != null)
        {
            _context.Units.Remove(existingUnit);
            await _context.SaveChangesAsync();
        }
    }

    // Products
    async Task<PaginatedResult<Product>> IProductRepository.GetAllProductsAsync(ProductFilters filters)
    {
       var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsQueryable();
        if (filters.Id?.Count > 0)
        {
            query = query.Where(p => filters.Id.Contains(p.Id));
        }
        if (filters.Name?.Count > 0)
        {
            foreach (var name in filters.Name)
            {
                var temp = name;
                query = query.Where(c => c.Name.StartsWith(temp));
            }
        }
        if (filters.CategoryId?.Count > 0)
        {
            query = query.Where(p => filters.CategoryId.Contains(p.CategoryId));
        }
        if (filters.UnitId?.Count > 0)
        {
            query = query.Where(p => filters.UnitId.Contains(p.UnitId));
        }
        if (filters.InternalCode?.Count > 0)
        {
            query = query.Where(p => p.InternalCode != null && filters.InternalCode.Contains(p.InternalCode));
        }
        if (filters.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filters.IsActive.Value);
        }
        if (filters.IsSellable.HasValue)
        {
            query = query.Where(p => p.IsSellable == filters.IsSellable.Value);
        }
        if (filters.IsPurchasable.HasValue)
        {
            query = query.Where(p => p.IsPurchasable == filters.IsPurchasable.Value);
        }
        if (filters.DoesExpire.HasValue)
        {
            query = query.Where(p => p.DoesExpire == filters.DoesExpire.Value);
        }

        var totalItems = query.Count();

        var products = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .AsNoTracking()
                .ToListAsync();

        return new PaginatedResult<Product>
        {
            Items = products,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalItems = totalItems
        };
    }

    async Task<Product?> IProductRepository.GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    async Task IProductRepository.AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    async Task IProductRepository.UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UnitId = product.UnitId;
            existingProduct.InternalCode = product.InternalCode;
            existingProduct.IsActive = product.IsActive;
            existingProduct.IsSellable = product.IsSellable;
            existingProduct.IsPurchasable = product.IsPurchasable;
            existingProduct.DoesExpire = product.DoesExpire;
            await _context.SaveChangesAsync();
        }
    }

    async Task IProductRepository.DeleteProductAsync(int id)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct != null)
        {
            _context.Products.Remove(existingProduct);
            await _context.SaveChangesAsync();
        }
    }
}