
using Data_Layer.commons;
using Data_Layer.Entities;
using Data_Layer.filters;

namespace Data_Layer.Interfaces;

public interface ICustomerRepository
{
    Task<PaginatedResult<Customer>> GetCustomersAsync(CustomerFilters filters);
    Task<Customer?> GetCustomerByIdAsync(int id);
    Task<Customer> CreateCustomerAsync(Customer customer);
}
