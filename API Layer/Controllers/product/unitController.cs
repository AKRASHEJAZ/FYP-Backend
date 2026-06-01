using Business_Layer.services;
using Data_Layer.filters;
using Business_Layer.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API_Layer.Controllers;

[ApiController]
[Route("api/unit")]
public class UnitController : ControllerBase
{
    private readonly UnitService _service;

    public UnitController(UnitService service)
    {
        _service = service;
    }

    [Authorize(Roles ="Admin, Cashier")]
    [HttpPost("Get")]
    async public Task<IActionResult> GetAll([FromBody] UnitFilters filters)
    {
        var data = await _service.GetAll(filters);
        return StatusCode(data.Code, data);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("Add")]
    async public Task<IActionResult> Add([FromBody] UpdateUnitDto unit)
    {
        var data = await _service.Add(unit);
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
    async public Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUnitDto dto)
    {
        var data = await _service.Update(id, dto);
        return StatusCode(data.Code, data);
    }
}
