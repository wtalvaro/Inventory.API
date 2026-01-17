using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/sessions")]
public class SalesSessionController : ControllerBase
{
    private readonly ISalesSessionService _sessionService;

    public SalesSessionController(ISalesSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<SalesSession>> Start([FromBody] SalesSession session)
    {
        var result = await _sessionService.StartAsync(session);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesSession>> Get(int id)
    {
        var session = await _sessionService.GetByIdAsync(id);
        if (session == null) return NotFound();
        return Ok(session);
    }

    [HttpPatch("{id}/stage")]
    public async Task<IActionResult> UpdateStage(int id, [FromQuery] string stage)
    {
        var success = await _sessionService.UpdateStageAsync(id, stage);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/finish")]
    public async Task<IActionResult> Finish(int id)
    {
        var total = await _sessionService.FinishAsync(id);
        if (total == null) return BadRequest("Não foi possível finalizar a sessão.");

        return Ok(new { message = "Venda finalizada!", totalVenda = total });
    }

    [HttpPost("{id}/abandon")]
    public async Task<IActionResult> Abandon(int id, [FromQuery] string reason)
    {
        var success = await _sessionService.AbandonAsync(id, reason);
        if (!success) return NotFound();
        return Ok(new { message = "Sessão encerrada (Abandono)." });
    }
}