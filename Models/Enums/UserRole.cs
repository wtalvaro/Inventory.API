namespace Inventory.API.Models.Enums;

public enum UserRole
{
    Vendedor,        // Vendedor: Foco total em vendas e atendimento ao cliente.
    Estoquista,   // Estoquista: Responsável pela entrada/saída física de produtos.
    Gerente,       // Gerente: Responsável pela unidade (estoque + vendas da loja).
    Coordenador,   // Coordenador: Visão regional de múltiplas unidades.
    Administrador  // Administrador: Controle global do ERP e setor de compras.
}