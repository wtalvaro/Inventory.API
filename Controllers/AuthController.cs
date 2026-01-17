using Inventory.API.Dtos;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }

        // --- O SEGREDO PARA ACABAR COM A TELA BRANCA ---
        // Criamos as Claims (identidade) que o Razor vai ler no Index.cshtml
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username ?? "Unknown"),
            new Claim(ClaimTypes.Role, response.Role ?? "Unknown"),
            new Claim("StoreId", response.StoreId?.ToString() ?? "0")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // O comando abaixo cria o Cookie no navegador do usuário
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true, // Mantém logado mesmo se fechar o navegador
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            });

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Este comando apaga o Cookie "RetailProAuth" do navegador
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}