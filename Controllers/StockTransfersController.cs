using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockTransfersController : ControllerBase
{
    private readonly IStockTransferService _transferService;

    public StockTransfersController(IStockTransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    [Authorize(Policy = "GerenteOuSuperior")]
    public async Task<IActionResult> GetAll() => Ok(await _transferService.GetAllAsync());

    [HttpPost]
    [Authorize(Policy = "ApenasCoordenador")] // Só o Boss ou o ADM autoriza saída da Matriz
    public async Task<IActionResult> Create(StockTransfer transfer)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        transfer.CreatedByUserId = userId;
        return Ok(await _transferService.CreateAsync(transfer));
    }

    [HttpPost("{id}/ship")]
    [Authorize(Policy = "ApenasCoordenador")]
    public async Task<IActionResult> Ship(int id)
    {
        var success = await _transferService.ShipAsync(id);
        return success ? Ok(new { message = "Mercadoria em trânsito!" }) : BadRequest("Erro ao despachar.");
    }

    [HttpPost("{id}/receive")]
    [Authorize(Policy = "GerenteOuSuperior")] // Gerente da Filial confirma a chegada
    public async Task<IActionResult> Receive(int id, [FromBody] List<StockTransferItem> items)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _transferService.ReceiveAsync(id, items, userId);
        return success ? Ok(new { message = "Recebimento concluído." }) : BadRequest("Erro no recebimento.");
    }
}