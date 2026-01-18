using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Exige estar logado para ver as dicas de venda
public class SalesStepsController : ControllerBase
{
    private readonly ISalesStepService _salesStepService;

    public SalesStepsController(ISalesStepService salesStepService)
    {
        _salesStepService = salesStepService;
    }

    // O Dashboard chamará este endpoint ao selecionar um produto
    [HttpGet("coach/{productId}")]
    public async Task<ActionResult<IEnumerable<SalesStep>>> GetTimeline(int productId)
    {
        var timeline = await _salesStepService.GetCoachTimelineAsync(productId);
        return Ok(timeline);
    }

    [HttpPost]
    [Authorize(Policy = "ApenasCoordenador")] // Apenas chefes criam o roteiro
    public async Task<ActionResult<SalesStep>> Create(SalesStep step)
    {
        var result = await _salesStepService.CreateAsync(step);
        return CreatedAtAction(nameof(GetTimeline), new { productId = result.ProductId }, result);
    }
}