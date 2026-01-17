using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems(int sessionId)
    {
        var items = await _cartService.GetItemsBySessionAsync(sessionId);
        return Ok(items);
    }

    [HttpPost("add")]
    public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItem newItem)
    {
        var result = await _cartService.AddOrUpdateItemAsync(newItem);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var success = await _cartService.RemoveItemAsync(id);
        if (!success) return NotFound("Item não encontrado no carrinho.");

        return NoContent();
    }

    [HttpDelete("session/{sessionId}/clear")]
    public async Task<IActionResult> ClearCart(int sessionId)
    {
        await _cartService.ClearCartAsync(sessionId);
        return NoContent();
    }
}