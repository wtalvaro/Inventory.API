using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize(Policy = "GerenteOuSuperior")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Store>>> GetActive()
    {
        var stores = await _storeService.GetAllActiveAsync();
        return Ok(stores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Store>> GetById(int id)
    {
        var store = await _storeService.GetByIdAsync(id);
        if (store == null) return NotFound("Loja não encontrada.");

        return Ok(store);
    }

    [HttpPost]
    public async Task<ActionResult<Store>> Create([FromBody] Store store)
    {
        var result = await _storeService.CreateAsync(store);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var success = await _storeService.DeactivateAsync(id);
        if (!success) return NotFound("Não foi possível desativar a loja solicitada.");

        return NoContent();
    }
}