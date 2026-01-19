namespace Inventory.API.Models.Dtos;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, string Username, string Role, int? StoreId);