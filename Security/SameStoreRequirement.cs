using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Inventory.API.Security;

// 1. O Requisito (Marcador)
public class SameStoreRequirement : IAuthorizationRequirement { }

// 2. O Handler (Lógica de Decisão)
public class SameStoreHandler : AuthorizationHandler<SameStoreRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SameStoreHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameStoreRequirement requirement)
    {
        // A. Extrai o StoreId do Token JWT (Claim)
        var userStoreIdClaim = context.User.FindFirst("StoreId")?.Value;

        // B. REGRA DE OURO: ACESSO TOTAL
        // Se for Coordenador OU se o StoreId for nulo/vazio/"0" no token,
        // o usuário tem permissão para acessar qualquer loja.
        if (context.User.IsInRole("Coordenador") ||
            string.IsNullOrEmpty(userStoreIdClaim) ||
            userStoreIdClaim == "0")
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // C. EXTRAÇÃO DO ID DA ROTA
        // Tenta capturar o parâmetro {storeId} da URL (ex: api/inventory/store/5)
        var routeValues = _httpContextAccessor.HttpContext?.Request.RouteValues;
        var routeStoreId = routeValues?["storeId"]?.ToString();

        // D. EXTRAÇÃO DA QUERY STRING (Fallback)
        // Se não estiver na rota, tenta buscar em ?storeId=5
        if (string.IsNullOrEmpty(routeStoreId))
        {
            routeStoreId = _httpContextAccessor.HttpContext?.Request.Query["storeId"].ToString();
        }

        // E. VALIDAÇÃO DE SEGURANÇA RESTRITA
        // Se a rota/query exige uma loja e o ID no Token do usuário for IGUAL, ele passa.
        // Se o usuário for um Gerente de outra loja, ele não cairá aqui e o acesso será negado.
        if (!string.IsNullOrEmpty(routeStoreId) && userStoreIdClaim == routeStoreId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}