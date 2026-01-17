using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/logs")]
public class InventoryLogsController : ControllerBase
{
    private readonly IInventoryLogService _logService;

    public InventoryLogsController(IInventoryLogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryLog>>> GetAll()
    {
        var logs = await _logService.GetAllLogsAsync();
        return Ok(logs);
    }

    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<IEnumerable<InventoryLog>>> GetBySku(string sku)
    {
        var logs = await _logService.GetLogsBySkuAsync(sku);
        if (!logs.Any()) return NotFound($"Nenhum registro encontrado para o SKU: {sku}");

        return Ok(logs);
    }

    [HttpGet("recent/{days}")]
    public async Task<ActionResult<IEnumerable<InventoryLog>>> GetRecent(int days)
    {
        var logs = await _logService.GetRecentLogsAsync(days);
        return Ok(logs);
    }
}