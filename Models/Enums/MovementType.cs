namespace Inventory.API.Models.Enums;

public enum MovementType
{
    Purchase,       // Entrada por Ordem de Compra
    Sale,           // Saída por Venda no PDV
    TransferIn,     // Entrada vinda de outra loja
    TransferOut,    // Saída enviada para outra loja
    AdjustmentIn,   // Ajuste manual positivo (ex: sobra de inventário)
    AdjustmentOut,  // Ajuste manual negativo (ex: avaria/quebra)
    Return          // Devolução de cliente
}