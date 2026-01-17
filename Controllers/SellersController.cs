using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/sellers")]
[Authorize(Policy = "ApenasCoordenador")] // Proteção de acesso mantida
public class SellersController : ControllerBase
{
    private readonly ISellerService _sellerService;

    public SellersController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seller>>> GetAll()
    {
        var sellers = await _sellerService.GetAllAsync();
        return Ok(sellers);
    }

    [HttpPost]
    public async Task<ActionResult<Seller>> Create(Seller seller)
    {
        var result = await _sellerService.CreateAsync(seller);

        // Retorna o local onde o recurso pode ser consultado (embora GetAll retorne a lista)
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }
}