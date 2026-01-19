using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockBatchesController : ControllerBase
{
    private readonly IStockBatchService _batchService;

    public StockBatchesController(IStockBatchService batchService)
    {
        _batchService = batchService;
    }

    [HttpGet("store/{storeId}/product/{productId}")]
    public async Task<IActionResult> GetInventoryBatches(int storeId, int productId)
    {
        var batches = await _batchService.GetBatchesByProductAsync(storeId, productId);
        return Ok(batches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var batch = await _batchService.GetByIdAsync(id);
        return batch == null ? NotFound() : Ok(batch);
    }

    // Endpoint para perdas ou vencimentos manuais
    [HttpPost("{id}/expire")]
    public async Task<IActionResult> Expire(int id)
    {
        var success = await _batchService.ExpireBatchAsync(id);
        return success ? Ok(new { message = "Lote invalidado com sucesso." }) : NotFound();
    }
}