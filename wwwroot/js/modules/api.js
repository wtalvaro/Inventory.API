/**
 * API.JS - Módulo de Comunicação Centralizado do RetailPro
 * Responsabilidade: Abstrair chamadas HTTP e tratar respostas globais.
 */

export const Api = {

    // --- NÚCLEO DE REQUISIÇÃO (Private-like helper) ---
    async _request(url, options = {}) {
        const defaultHeaders = { 'Content-Type': 'application/json' };
        options.headers = { ...defaultHeaders, ...options.headers };

        try {
            const res = await fetch(url, options);

            // Tratamento global de Autenticação
            if (res.status === 401) {
                this.handleUnauthorized();
                return null;
            }

            if (!res.ok) {
                const errorText = await res.text();
                console.error(`🔴 Erro API [${res.status}] em ${url}:`, errorText);
                return null;
            }

            // Retorna null se não houver conteúdo (NoContent)
            if (res.status === 204) return true;

            return await res.json();
        } catch (err) {
            console.error(`🔴 Falha de Conexão em ${url}:`, err);
            return null;
        }
    },

    // --- 1. INVENTÁRIO (Logística Local) ---
    async getInventory(storeId) {
        return await this._request(`/api/StoreInventory/catalog/${storeId}`) || [];
    },

    async updateStock(id, newQuantity, reason) {
        // Endpoint: [HttpPut("StoreInventory/{id}/adjust")]
        const url = `/api/StoreInventory/${id}/adjust?quantity=${newQuantity}&reason=${encodeURIComponent(reason)}`;
        const result = await this._request(url, {
            method: 'PUT',
            body: JSON.stringify({ quantity: newQuantity })
        });
        return result !== null;
    },

    // --- 2. COMPRAS (Purchase Orders - ERP Core) ---
    async getPurchaseOrders() {
        return await this._request('/api/PurchaseOrders') || [];
    },

    async getPurchaseOrderById(id) {
        return await this._request(`/api/PurchaseOrders/${id}`);
    },

    async createPurchaseOrder(data) {
        return await this._request('/api/PurchaseOrders', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    },

    async receivePurchaseOrder(id) {
        const res = await this._request(`/api/PurchaseOrders/${id}/receive`, { method: 'POST' });
        return res !== null;
    },

    // --- 3. ITENS DA ORDEM DE COMPRA ---
    async addOrderItem(item) {
        return await this._request('/api/PurchaseOrderItems', {
            method: 'POST',
            body: JSON.stringify(item)
        });
    },

    async deleteOrderItem(id) {
        const res = await this._request(`/api/PurchaseOrderItems/${id}`, { method: 'DELETE' });
        return res !== null;
    },

    // --- 4. INTELIGÊNCIA E AUXILIARES ---
    async getSalesCoachTimeline(productId) {
        return await this._request(`/api/SalesSteps/coach/${productId}`) || [];
    },

    async searchProducts(query) {
        if (!query || query.length < 2) return [];
        return await this._request(`/api/Products/search?q=${encodeURIComponent(query)}`) || [];
    },

    async getSuppliers() {
        return await this._request('/api/Suppliers') || [];
    },

    async getStores() {
        // Útil para o Coordenador escolher o destino da compra
        return await this._request('/api/Stores') || [];
    },

    // --- 5. TELEMETRIA (Mock/Previsão de Implementação) ---
    async getTelemetryData() {
        // Atualmente simulado para não gerar erros de console enquanto o serviço é construído
        return new Promise((resolve) => {
            setTimeout(() => {
                resolve({
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
                });
            }, 300);
        });
    },

    // --- 6. SESSÃO E SEGURANÇA ---
    async logout() {
        try {
            await fetch('/api/auth/logout', { method: 'POST' });
        } finally {
            sessionStorage.clear();
            localStorage.clear();
            window.location.replace('/');
        }
    },

    handleUnauthorized() {
        console.warn("Sessão expirada. Redirecionando...");
        window.location.href = '/';
    }
};