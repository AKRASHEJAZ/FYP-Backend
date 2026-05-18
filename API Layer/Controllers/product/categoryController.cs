using Business_Layer.services;
using Data_Layer.filters;
using Business_Layer.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API_Layer.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController : Controller
{
    private readonly CategoryService _service;

    public CategoryController(CategoryService service)
    {
        _service = service;
    }

    [Authorize(Roles ="Admin, Cashier")]
    [HttpPost("Get")]
    async public Task<IActionResult> GetAll([FromBody] CategoryFilters filters)
    {
        var data = await _service.GetAllCategoriesAsync(filters);
        return StatusCode(data.Code, data);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("Add")]
    async public Task<IActionResult> Add([FromBody] UpdateCategoryDto category)
    {
        var data = await _service.AddCategoryAsync(category);
        return StatusCode(data.Code, data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    async public Task<IActionResult> Delete([FromRoute] int id)
    {
        var data = await _service.Delete(id);
        return StatusCode(data.Code, data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("Update/{id}")]
    async public Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto)
    {
        var data = await _service.Update(id, dto);
        return StatusCode(data.Code, data);
    }
}
