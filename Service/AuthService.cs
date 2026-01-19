using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net; // Garanta que instalou o pacote: dotnet add package BCrypt.Net-Next

namespace Inventory.API.Services;

public class AuthService : IAuthService
{
    private readonly InventoryDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(InventoryDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) return null;

        // Compara a senha digitada com o Hash do banco de dados
        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return isValid ? user : null;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? "chave-secreta-muito-longa-e-segura";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // IMPORTANTE: Convertemos o Enum Role para string para o sistema de autorização entender
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("StoreId", user.StoreId?.ToString() ?? "0")
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}