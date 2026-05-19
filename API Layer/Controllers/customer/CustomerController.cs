using Microsoft.AspNetCore.Mvc;
using Business_Layer.services;
using Data_Layer.filters;
using Business_Layer.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace API_Layer.Controllers;

[ApiController]
[Route("api/customer")]

public class CustomerController : Controller
{
    private readonly CustomerService _service;

    public CustomerController(CustomerService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin, Cashier")]
    [HttpPost("get")]
    public IActionResult GetCustomers([FromBody] CustomerFilters filters)
    {
        var data = _service.GetCustomersAsync(filters).Result;
        return StatusCode(data.Code, data);
    }

    [Authorize(Roles = "Admin, Cashier")]
    [HttpPost("add")]
    public IActionResult AddCustomer([FromBody] CreateCustomerDto customer)
    {
        var data = _service.CreateCustomerAsync(customer).Result;
        return StatusCode(data.Code, data);
    }
}


