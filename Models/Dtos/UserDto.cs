namespace Inventory.API.Models.Dtos;

// DTO para Criação: Recebe a senha pura para ser processada
public record UserCreateDto(
    string Username,
    string Password,
    string Role,
    int? StoreId
);

// DTO para Leitura: Retorno seguro para o Frontend (sem senha)
public record UserReadDto(
    int Id,
    string Username,
    string Role,
    int? StoreId
);

// DTO para Atualização: Permite mudar dados sem mexer na senha obrigatoriamente
public record UserUpdateDto(
    string Username,
    string Role,
    int? StoreId
);