using Business_Layer.services;
using Data_Layer.filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers;

[ApiController]
[Route("api/report")]
public class ReportController : ControllerBase
{
    private readonly ReportService _service;

    public ReportController(ReportService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("GetCount")]
    public async Task<IActionResult> GetCountInfo()
    {
        var data = await _service.GetCountData();
        return StatusCode(data.Code, data);
    }

    [Authorize]
    [HttpPost("GetExpiredProducts")]
    public async Task<IActionResult> GetExpiredProducts([FromBody] ExpiryReportFilters filters)
    {
        var data = await _service.GetExpiredProduct(filters);
        return StatusCode(data.Code, data);
    }
}
