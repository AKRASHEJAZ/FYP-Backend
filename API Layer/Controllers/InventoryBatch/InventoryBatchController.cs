using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers;


[ApiController]
[Route("api/stock")]

public class InventoryBatchController : Controller
{
    private readonly InventoryBatchService _service;

    public InventoryBatchController(InventoryBatchService service)
    {
        _service = service;
    }

    [HttpPost("get")]
    public async Task<IActionResult> GetInventoryBatch([FromBody] InventoryBatchFilters filters)
    {
        var data = await _service.GetAllInventoryBatchesAsync(filters);
        return StatusCode(data.Code, data.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("add")]
    public async Task<IActionResult> AddInventoryBatch([FromBody] AddInventoryBatchDto batch)
    {
        var data = await _service.AddInventoryBatchAsync(batch);
        return StatusCode(data.Code, data.Data);
    }
}