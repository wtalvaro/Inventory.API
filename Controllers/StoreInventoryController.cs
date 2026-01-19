using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreInventoryController : ControllerBase
{
    private readonly IStoreInventoryService _inventoryService;

    public StoreInventoryController(IStoreInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("catalog/{storeId}")]
    [Authorize(Policy = "VendedorOuSuperior")]
    public async Task<IActionResult> GetCatalog(int storeId)
    {
        var items = await _inventoryService.GetCatalogAsync(storeId);
        return Ok(items);
    }

    [HttpGet("low-stock/{storeId}")]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<IActionResult> GetAlerts(int storeId)
    {
        var items = await _inventoryService.GetItemsBelowMinimumAsync(storeId);
        return Ok(items);
    }

    [HttpPut("{id}/adjust")]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<IActionResult> Adjust(int id, [FromQuery] int quantity, [FromQuery] string reason)
    {
        var result = await _inventoryService.AdjustStockAsync(id, quantity, reason);
        if (result == null) return NotFound();
        return Ok(result);
    }
}