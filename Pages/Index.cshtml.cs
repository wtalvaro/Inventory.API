using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Inventory.API.Services.Interfaces;
using Inventory.API.Dtos;
using Inventory.API.Models; // Necessário para StoreInventory

namespace Inventory.API.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IInventoryService _inventoryService;

        // Propriedade que o HTML (Index.cshtml) usará para montar o select das lojas
        public IEnumerable<StoreInventory> Stores { get; set; } = new List<StoreInventory>();

        public IndexModel(IAuthService authService, IInventoryService inventoryService)
        {
            _authService = authService;
            _inventoryService = inventoryService;
        }

        // --- 1. CARREGAMENTO DA PÁGINA (Ponto de entrada) ---
        public async Task OnGetAsync()
        {
            // O 'is { IsAuthenticated: true }' verifica se identity não é nulo 
            // E se a propriedade IsAuthenticated é verdadeira, tudo em um passo só.
            if (User?.Identity is { IsAuthenticated: true } && User.IsInRole("Coordenador"))
            {
                var catalog = await _inventoryService.GetCatalogAsync(0);

                if (catalog != null)
                {
                    Stores = catalog
                        .GroupBy(x => x.StoreId)
                        .Select(g => g.First())
                        .ToList();
                }
            }
        }

        // --- 2. PROCESSAMENTO DO LOGIN (Envio do formulário) ---
        public async Task<IActionResult> OnPostLoginAsync(string username, string password)
        {
            // Criando a requisição usando o construtor do seu DTO
            var loginRequest = new LoginRequest(username, password);
            var response = await _authService.LoginAsync(loginRequest);

            if (response == null)
            {
                // Adiciona erro que será exibido no asp-validation-summary
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return Page();
            }

            // Criando as Claims que definem a identidade do usuário no sistema
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, response.Role ?? "Vendedor"),
                new Claim("StoreId", response.StoreId?.ToString() ?? "0")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Gerando o Cookie de Autenticação persistente (mantém logado ao fechar aba)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

            // Redireciona para o GET da própria página para carregar o Dashboard
            return RedirectToPage();
        }
    }
}