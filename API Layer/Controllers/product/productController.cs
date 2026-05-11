using Business_Layer.services;
using Data_Layer.filters;
using Business_Layer.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API_Layer.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : Controller
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
    {
        _service = service;
    }

    [Authorize(Roles ="Admin, cashier")]
    [HttpPost("Get")]
    async public Task<IActionResult> GetAll([FromBody] ProductFilters filters)
    {
        var data = await _service.GetAllProductsAsync(filters);
        return StatusCode(data.Code, data.Data);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("Add")]
    async public Task<IActionResult> Add([FromBody] UpdateProductDto product)
    {
        var data = await _service.Add(product);
        return StatusCode(data.Code, data.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    async public Task<IActionResult> Delete([FromRoute] int id)
    {
        var data = await _service.Delete(id);
        return StatusCode(data.Code, data.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("Update/{id}")]
    async public Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
    {
        var data = await _service.Update(id, dto);
        return StatusCode(data.Code, data.Data);
    }
}
