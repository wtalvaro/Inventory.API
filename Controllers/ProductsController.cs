using Microsoft.AspNetCore.Mvc;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    // Injeção de Dependência: O Controller recebe o serviço pronto para uso
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // 1. LISTAGEM COM FILTROS AVANÇADOS
    // GET: api/Products?category=Vestuário&gender=Masculino
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? subCategory,
        [FromQuery] string? gender,
        [FromQuery] string? targetAudience)
    {
        var products = await _productService.GetAllAsync(category, subCategory, gender, targetAudience);
        return Ok(products);
    }

    // 2. BUSCA POR ID
    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound(new { message = "Produto não encontrado." });

        return Ok(product);
    }

    // 3. CRIAÇÃO DE PRODUTO
    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        var created = await _productService.CreateAsync(product);

        // Retorna o status 201 (Created) e o link para ver o item criado
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // 4. ATUALIZAÇÃO COMPLETA
    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        var success = await _productService.UpdateAsync(id, product);

        if (!success) return BadRequest(new { message = "Não foi possível atualizar o produto. Verifique o ID." });

        return NoContent(); // Status 204: Sucesso, mas sem conteúdo no corpo
    }

    // 5. EXCLUSÃO
    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);

        if (!success) return NotFound(new { message = "Produto não encontrado para exclusão." });

        return NoContent();
    }
}