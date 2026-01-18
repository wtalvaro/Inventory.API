/**
 * API.JS - Módulo de Comunicação Centralizado
 */
export const Api = {
    // 1. INVENTÁRIO
    async getInventory(storeId) {
        try {
            const res = await fetch(`/api/inventory/store/${storeId}`);
            if (res.status === 401) return this.handleUnauthorized();
            return await res.json();
        } catch (err) {
            console.error("Erro ao buscar inventário:", err);
            throw err;
        }
    },

    async updateStock(id, change) {
        try {
            const res = await fetch(`/api/inventory/${id}/stock?change=${change}`, {
                method: 'PATCH'
            });
            if (res.status === 401) return this.handleUnauthorized();
            return res.ok;
        } catch (err) {
            console.error("Erro ao atualizar estoque:", err);
            return false;
        }
    },

    // 2. COACH DE VENDAS
    async getSalesCoachTimeline(productId) {
        try {
            const res = await fetch(`/api/SalesSteps/coach/${productId}`);
            if (res.status === 401) return this.handleUnauthorized();
            if (!res.ok) return [];
            return await res.json();
        } catch (err) {
            console.error("Erro ao buscar timeline do coach:", err);
            return [];
        }
    },

    // 3. TELEMETRIA (Apenas Coordenadores)
    async getTelemetryData() {
        try {
            const res = await fetch('/api/telemetry/dashboard');
            if (res.status === 401) return this.handleUnauthorized();

            if (!res.ok) {
                // Se o backend não responder, usamos dados de teste (Mock)
                return this.getMockTelemetryData();
            }
            return await res.json();
        } catch (err) {
            console.warn("API Offline, usando dados simulados.");
            return this.getMockTelemetryData();
        }
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

    // 4. AUTENTICAÇÃO
    async logout() {
        try {
            await fetch('/api/auth/logout', { method: 'POST' });
        } finally {
            window.location.reload();
        }
    },

    // Helper para tratar expiração de sessão
    handleUnauthorized() {
        console.warn("Sessão expirada. Redirecionando...");
        window.location.reload();
    }
};