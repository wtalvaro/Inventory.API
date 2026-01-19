using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Inventory.API.Services.Interfaces;
using Inventory.API.Models;

namespace Inventory.API.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IStoreInventoryService _inventoryService;

        // Propriedade para montar o select de lojas (para Coordenadores)
        public IEnumerable<StoreInventory> Stores { get; set; } = new List<StoreInventory>();

        public IndexModel(IAuthService authService, IStoreInventoryService inventoryService)
        {
            _authService = authService;
            _inventoryService = inventoryService;
        }

        public async Task OnGetAsync()
        {
            // Se já estiver logado e for coordenador, carrega as lojas
            if (User?.Identity is { IsAuthenticated: true } && User.IsInRole("Coordenador"))
            {
                var catalog = await _inventoryService.GetCatalogAsync(0);
                if (catalog != null)
                {
                    Stores = catalog.GroupBy(x => x.StoreId).Select(g => g.First());
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Preencha todos os campos.");
                return Page();
            }

            // 1. Chamada CORRIGIDA: ValidateUserAsync em vez de LoginAsync
            // O retorno agora é o objeto 'User' ou 'null'
            var user = await _authService.ValidateUserAsync(username, password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return Page();
            }

            // 2. Montagem das Claims CORRIGIDA:
            // Usamos .ToString() no Role (Enum) para evitar o erro de BinaryReader
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()), // RESOLVE O ERRO CS1503
                new Claim("StoreId", user.StoreId?.ToString() ?? "0")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Efetua o Login via Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    // 1. Torna o cookie volátil (memória RAM apenas)
                    IsPersistent = false,

                    // 2. Define um tempo máximo de vida, mesmo se o navegador estiver aberto
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),

                    // 3. Permite que o tempo seja renovado se o usuário estiver ativo
                    AllowRefresh = true
                });

            return RedirectToPage();
        }
    }
}