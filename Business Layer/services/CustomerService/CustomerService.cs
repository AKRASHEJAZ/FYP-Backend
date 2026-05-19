using Data_Layer.Interfaces;
using Data_Layer.filters;
using Data_Layer.Entities;
using Data_Layer.commons;
using Business_Layer.Common;
using Business_Layer.DTOS;

namespace Business_Layer.services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ApiResponse<PaginatedResult<CustomerDto>>> GetCustomersAsync(CustomerFilters filters)
    {
        var response = await _customerRepository.GetCustomersAsync(filters);
        var customers = response.Items;

        if(customers == null || !customers.Any())
        {
            return ApiResponse<PaginatedResult<CustomerDto>>.Fail("No customers found.");
        }

        var paginatedDto = new PaginatedResult<CustomerDto>
        {
            Items = customers.Select(c => new CustomerDto(c)).ToList(),
            Page = response.Page,
            PageSize = response.PageSize,
            TotalItems = response.TotalItems
        };

        return ApiResponse<PaginatedResult<CustomerDto>>.Success(paginatedDto);
    }

    public async Task<ApiResponse<CustomerDto>> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return ApiResponse<CustomerDto>.Fail("Customer not found.");
        }
        return ApiResponse<CustomerDto>.Success(new CustomerDto(customer));
    }

    public async Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        var newCustomer = new Customer
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Address = createDto.Address,
            Phone = createDto.Phone,
            IsWalkIn = createDto.IsWalkIn,
            CreatedAt = DateTime.UtcNow
        };
        var createdCustomer = await _customerRepository.CreateCustomerAsync(newCustomer);
        return ApiResponse<CustomerDto>.Success(new CustomerDto(createdCustomer));
    }   
}
