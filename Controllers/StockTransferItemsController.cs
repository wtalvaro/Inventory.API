using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "GerenteOuSuperior")] // Itens podem ser visualizados por gerentes
public class StockTransferItemsController : ControllerBase
{
    private readonly IStockTransferItemService _itemService;

    public StockTransferItemsController(IStockTransferItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("transfer/{transferId}")]
    public async Task<IActionResult> GetByTransfer(int transferId)
    {
        return Ok(await _itemService.GetByTransferIdAsync(transferId));
    }

    [HttpPost]
    [Authorize(Policy = "ApenasCoordenador")] // Apenas o Coordenador monta a carga
    public async Task<IActionResult> Create(StockTransferItem item)
    {
        try
        {
            var result = await _itemService.AddAsync(item);
            return CreatedAtAction(nameof(GetByTransfer), new { transferId = result.StockTransferId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _itemService.DeleteAsync(id);
        return success ? NoContent() : BadRequest("Item não encontrado ou transferência já processada.");
    }
}