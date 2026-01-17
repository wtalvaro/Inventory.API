using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.API.Data;
using Inventory.API.Dtos;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.API.Services;

public class AuthService : IAuthService
{
    private readonly InventoryDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(InventoryDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // Verificação básica de senha (em produção, use BCrypt)
        if (user == null || user.PasswordHash != request.Password)
        {
            return null;
        }

        var token = GenerateJwtToken(user);

        // Retorna o StoreId (que pode ser null para o Coordenador)
        return new LoginResponse(token, user.Username, user.Role, user.StoreId);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key missing");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 1. Criamos a lista de Claims de forma dinâmica
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 2. Lógica de StoreId Nulo (Acesso Global)
        // Se o StoreId tiver valor, adicionamos a claim. 
        // Se for null (Coordenador), mandamos "0" ou simplesmente não mandamos.
        // O nosso SameStoreHandler já trata string vazia ou "0" como ACESSO TOTAL.
        claims.Add(new Claim("StoreId", user.StoreId?.ToString() ?? "0"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "RetailProAPI",
            audience: _configuration["Jwt:Audience"] ?? "RetailProFrontend",
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}