using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _orderService;

    public PurchaseOrdersController(IPurchaseOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(PurchaseOrder order)
    {
        var result = await _orderService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost("{id}/receive")]
    public async Task<IActionResult> Receive(int id)
    {
        var success = await _orderService.ReceiveOrderAsync(id);
        if (!success) return BadRequest("Ordem não encontrada ou já recebida.");
        return Ok(new { message = "Estoque atualizado e lotes criados com sucesso!" });
    }
}