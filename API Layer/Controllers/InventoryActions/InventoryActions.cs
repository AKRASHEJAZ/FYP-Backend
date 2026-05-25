using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.DTOS;
using Business_Layer.services;
using Data_Layer.filters;


namespace API_Layer.Controllers;

[ApiController]
[Route("api/InventoryAction")]
public class InventoryActions : ControllerBase
{
    private readonly InventoryActionService _inventoryActionService;

    public InventoryActions(InventoryActionService inventoryActionService)
    {
        _inventoryActionService = inventoryActionService;
    }

    [Authorize]
    [HttpPost("CreateSale")]
    public async Task<IActionResult> CreateSale([FromBody] AddSaleDto dto)
    {
        var result = await _inventoryActionService.CreateSaleAsync(dto);
        
        return StatusCode(result.Code, result);
    }

    [Authorize]
    [HttpPost("GetSale")]
    public async Task<IActionResult> GetSale([FromBody] SaleFilters filters)
    {
        var results = await _inventoryActionService.GetSalesAsync(filters);
        return StatusCode(results.Code, results);
    }
}