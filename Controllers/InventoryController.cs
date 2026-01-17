using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("store/{storeId}")]
    [Authorize(Policy = "VendedorOuSuperior")]
    public async Task<ActionResult<IEnumerable<StoreInventory>>> GetCatalog(int storeId)
    {
        var items = await _inventoryService.GetCatalogAsync(storeId);
        return Ok(items);
    }

    [HttpGet("store/{storeId}/sku/{sku}")]
    public async Task<ActionResult<StoreInventory>> GetBySku(int storeId, string sku)
    {
        var item = await _inventoryService.GetBySkuAsync(storeId, sku);
        if (item == null) return NotFound("SKU não encontrado nesta unidade.");
        return Ok(item);
    }

    [HttpPut("item/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StoreInventory updateData)
    {
        var result = await _inventoryService.UpdateItemAsync(id, updateData);
        if (result == null) return NotFound("Item não encontrado.");
        return Ok(result);
    }

    [HttpGet("store/{storeId}/low-stock/{threshold}")]
    public async Task<ActionResult<IEnumerable<StoreInventory>>> GetLowStock(int storeId, int threshold)
    {
        var items = await _inventoryService.GetLowStockAsync(storeId, threshold);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<StoreInventory>> Create([FromBody] StoreInventory newItem)
    {
        var result = await _inventoryService.AddToInventoryAsync(newItem);
        return CreatedAtAction(nameof(GetBySku), new { storeId = result.StoreId, sku = result.SKU }, result);
    }
}