using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderItemsController : ControllerBase
{
    private readonly IPurchaseOrderItemService _itemService;

    public PurchaseOrderItemsController(IPurchaseOrderItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(PurchaseOrderItem item)
    {
        try
        {
            var result = await _itemService.AddItemToOrderAsync(item);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var success = await _itemService.RemoveItemAsync(id);
        return success ? NoContent() : BadRequest("Não foi possível remover o item (Ordem finalizada ou item inexistente).");
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var items = await _itemService.GetItemsByOrderIdAsync(orderId);
        return Ok(items);
    }
}