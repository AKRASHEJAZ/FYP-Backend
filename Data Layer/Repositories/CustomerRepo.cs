
using Data_Layer.commons;
using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.filters;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Repositories;

public class CustomerRepo : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        return customer;
    }

    public async Task<PaginatedResult<Customer>> GetCustomersAsync(CustomerFilters filters)
    {
        var query = _context.Customers.AsQueryable();

        if(filters.Id > 0)
        {
            query = query.Where(c => c.Id == filters.Id);
        }

        if (!string.IsNullOrEmpty(filters.Name))
        {
            query = query.Where(c => c.Name.StartsWith(filters.Name));
        }

        if (!string.IsNullOrEmpty(filters.Email))
        {
            query = query.Where(c => c.Email!.Contains(filters.Email));
        }

        var totalItems = await query.CountAsync();
        var customers = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PaginatedResult<Customer>
        {
            Items = customers,
            TotalItems = totalItems,
            Page = filters.Page,
            PageSize = filters.PageSize
        };
    }
}