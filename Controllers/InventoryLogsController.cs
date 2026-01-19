using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryLogsController : ControllerBase
{
    private readonly IInventoryLogService _logService;

    public InventoryLogsController(IInventoryLogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<IActionResult> GetAll([FromQuery] int? storeId)
    {
        var logs = await _logService.GetLogsAsync(storeId);
        return Ok(logs);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var logs = await _logService.GetLogsByProductAsync(productId);
        return Ok(logs);
    }
}