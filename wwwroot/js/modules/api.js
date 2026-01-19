/**
 * API.JS - Módulo de Comunicação Centralizado
 */
export const Api = {
    // 1. INVENTÁRIO (Alinhado com StoreInventoryController)
    async getInventory(storeId) {
        try {
            // O Backend espera: /api/inventory/store/{storeId}
            const res = await fetch(`/api/StoreInventory/catalog/${storeId}`);

            if (res.status === 401) return this.handleUnauthorized();

            // SEGURANÇA: Se não for 200 OK, não tenta ler JSON
            if (!res.ok) {
                console.warn(`API retornou erro ${res.status} para a loja ${storeId}`);
                return [];
            }

            return await res.json();
        } catch (err) {
            console.error("Erro ao buscar inventário:", err);
            return [];
        }
    },

    // Ajustado para usar o endpoint [HttpPut("item/{id}")] do seu Controller
    async updateStock(id, currentQuantity, change) {
        try {
            const newQuantity = currentQuantity + change;

            // O seu Controller espera um objeto StoreInventory no Body
            const res = await fetch(`/api/StoreInventory/${id}/adjust?quantity=${newQuantity}&reason=${encodeURIComponent(reason)}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ quantity: newQuantity })
            });

            if (res.status === 401) return this.handleUnauthorized();
            return res.ok;
        } catch (err) {
            console.error("Erro ao atualizar estoque:", err);
            return false;
        }
    },

    // 2. COACH DE VENDAS (Alinhado com SalesStepsController)
    async getSalesCoachTimeline(productId) {
        try {
            // Ajustado para a rota correta do seu Controller
            const res = await fetch(`/api/SalesSteps/coach/${productId}`);
            if (!res.ok) return [];
            return await res.json();
        } catch (err) {
            return [];
        }
    },

    // 3. TELEMETRIA (Mock de segurança para evitar erro 404 no console)
    async getTelemetryData() {
        console.info("Telemetria: Endpoint desativado no JS. Usando dados locais.");
        return this.getMockTelemetryData();
    },

    getMockTelemetryData() {
        return {
            totalSales: 45250.80,
            activeSessions: 12,
            outOfStockCount: 5,
            averageTicket: 185.50,
            storePerformances: [
                { storeName: "Centro-RJ", stockLevel: 1250, salesGrowth: 15 },
                { storeName: "Shopping Paulista", stockLevel: 840, salesGrowth: -5 },
                { storeName: "Filial Curitiba", stockLevel: 2100, salesGrowth: 8 },
                { storeName: "Recife Antigo", stockLevel: 450, salesGrowth: 12 }
            ]
        };
    },

    async logout() {
        try {
            // Chama o servidor para invalidar o cookie
            await fetch('/api/auth/logout', { method: 'POST' });
        } finally {
            // Limpa estados locais se houver (localStorage, sessionStorage)
            sessionStorage.clear();
            localStorage.removeItem('RetailPro_LastUser');

            // Redireciona e força um reload completo
            window.location.replace('/');
        }
    },

    handleUnauthorized() {
        window.location.href = '/';
    }
};