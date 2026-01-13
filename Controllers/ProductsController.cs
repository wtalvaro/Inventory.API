using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.API.Data;
using Inventory.API.Models;
using System.Text;

namespace Inventory.API.Controllers;

/// <summary>
/// Controlador responsável pela gestão de produtos, simulação de hardware (LED) e inteligência de estoque.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public ProductsController(InventoryDbContext context)
    {
        _context = context;
    }

    #region Consultas e Inteligência

    /// <summary>
    /// Lista todos os produtos cadastrados no sistema.
    /// </summary>
    /// <returns>Uma lista de objetos do tipo Product.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// Busca um produto pelo SKU, aciona o LED e retorna inteligência de vendas (sugestões e dicas).
    /// </summary>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBySku(string sku)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());

        if (product == null) return NotFound("Produto não localizado.");

        // --- CORREÇÃO AQUI: Definindo a variável timeline ---
        // Se o SalesTimeline for nulo, retornamos uma lista vazia para não quebrar o front
        var timeline = product.SalesTimeline ?? new List<SalesStep>();

        // Busca sugestões (Cross-Sell) incluindo a nova coluna CrossSellAdvantage
        var suggestions = await _context.Products
            .Where(p => product.RelatedSkus.Contains(p.SKU))
            .Select(s => new
            {
                s.SKU,
                s.Name,
                s.Price,
                s.Quantity,
                s.Category,
                // Certifique-se de que essa coluna já foi criada no banco/model
                CrossSellAdvantage = s.CrossSellAdvantage
            })
            .ToListAsync();

        return Ok(new
        {
            Details = product,
            Suggestions = suggestions,
            Timeline = timeline // Agora a variável existe!
        });
    }

    /// <summary>
    /// Retorna o ranking dos 5 produtos mais buscados (maior incidência de acionamento de LED).
    /// </summary>
    [HttpGet("stats/top-searched")]
    public async Task<IActionResult> GetTopSearched()
    {
        var stats = await _context.InventoryLogs
            .GroupBy(l => l.ProductName)
            .Select(g => new { Name = g.Key, TotalSearches = g.Count() })
            .OrderByDescending(x => x.TotalSearches)
            .Take(5)
            .ToListAsync();

        return Ok(stats);
    }

    /// <summary>
    /// Busca todas as variantes (tamanhos/cores) de um modelo de produto pelo nome.
    /// Útil para exibir uma grade de tamanhos no Dashboard.
    /// </summary>
    [HttpGet("model/{name}")]
    public async Task<IActionResult> GetModelDetails(string name)
    {
        var variants = await _context.Products
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .ToListAsync();

        if (!variants.Any()) return NotFound("Nenhum modelo encontrado com este nome.");

        var modelView = new
        {
            Name = variants.First().Name,
            Category = variants.First().Category,
            BasePrice = variants.First().Price,
            Aisle = variants.First().Aisle,
            Shelf = variants.First().Shelf, // ADICIONE ESTA LINHA AQUI
            AvailableSizes = variants.Select(v => new
            {
                v.SKU,
                v.Quantity,
                Tamanho = v.Specifications.ContainsKey("Tamanho") ? v.Specifications["Tamanho"] : "N/A",
                v.LedPin,
                v.Shelf // Adicionado aqui também caso mude por tamanho
            }).OrderBy(v => v.Tamanho).ToList()
        };

        return Ok(modelView);
    }

    #endregion

    #region Operações de Estoque

    /// <summary>
    /// Cadastra um novo produto no inventário.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBySku), new { sku = product.SKU }, product);
    }

    /// <summary>
    /// Realiza a baixa de X unidades no estoque por venda.
    /// </summary>
    [HttpPatch("sell/{sku}")]
    public async Task<IActionResult> SellProduct(string sku, [FromQuery] int quantity = 1)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());

        if (product == null) return NotFound("Produto não cadastrado.");

        // Validação de estoque para a quantidade solicitada
        if (product.Quantity < quantity)
            return BadRequest($"Estoque insuficiente! Disponível: {product.Quantity}");

        // Abate o total de uma só vez (Operação Atômica)
        product.Quantity -= quantity;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Venda realizada!",
            novoEstoque = product.Quantity,
            sku = product.SKU
        });
    }

    /// <summary>
    /// Reabastece o estoque de um produto com uma quantidade específica.
    /// </summary>
    [HttpPatch("restock/{sku}")]
    public async Task<IActionResult> RestockProduct(string sku, [FromQuery] int quantity)
    {
        if (quantity <= 0) return BadRequest("A quantidade deve ser maior que zero.");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());

        if (product == null) return NotFound("Produto não encontrado.");

        product.Quantity += quantity;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Estoque reabastecido!", novoEstoque = product.Quantity });
    }

    #endregion

    #region Configurações e Exportação

    /// <summary>
    /// Atualiza o tempo de duração que o LED permanecerá aceso para um SKU específico.
    /// </summary>
    [HttpPut("config-led/{sku}")]
    public async Task<IActionResult> UpdateLedConfig(string sku, [FromQuery] int seconds)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());

        if (product == null) return NotFound();

        product.LedDurationSeconds = seconds;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Tempo configurado para {seconds}s" });
    }

    /// <summary>
    /// Gera e exporta um arquivo CSV com o estado atual de todo o inventário.
    /// </summary>
    [HttpGet("export/inventory")]
    public async Task<IActionResult> ExportInventory()
    {
        var products = await _context.Products.ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("SKU;Nome;Quantidade;Corredor;Prateleira;Pino LED");

        foreach (var p in products)
        {
            csv.AppendLine($"{p.SKU};{p.Name};{p.Quantity};{p.Aisle};{p.Shelf};{p.LedPin}");
        }

        var buffer = Encoding.UTF8.GetBytes(csv.ToString());
        string fileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.csv";

        return File(buffer, "text/csv", fileName);
    }

    /// <summary>
    /// Atualiza o roteiro (timeline) de vendas de um produto específico.
    /// </summary>
    [HttpPut("update-timeline/{sku}")]
    public async Task<IActionResult> UpdateTimeline(string sku, [FromBody] List<SalesStep> newTimeline)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower());

        if (product == null)
            return NotFound("Produto não encontrado para atualizar o roteiro.");

        // Atualiza a lista de passos no banco (PostgreSQL tratará como JSONB)
        product.SalesTimeline = newTimeline;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = $"Roteiro de vendas do produto {product.Name} atualizado com sucesso!",
            stepsCount = newTimeline.Count
        });
    }

    #endregion
}