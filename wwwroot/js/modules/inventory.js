/**
 * INVENTORY.JS - Lógica de Negócio e Renderização de Estoque
 */
import { Api } from './api.js';
import { UI } from './ui.js';

export const Inventory = {
    currentItems: [],

    async load() {
        UI.toggleLoading(true);

        // LÓGICA CORRIGIDA:
        // 1. Tenta pegar o elemento de filtro (só existe para Coordenador)
        const filterEl = document.getElementById('store-filter');

        // 2. Define o ID: Se o filtro existir, usa o valor dele. 
        // Se não (Vendedor), usa o storeId do ServerState.
        let sId = filterEl
            ? filterEl.value
            : (ServerState.storeId || 0);

        console.log(`Inventory: Carregando dados para a loja ${sId}`);

        try {
            const data = await Api.getInventory(sId);
            this.currentItems = data || [];
            this.render();
        } catch (err) {
            console.error("Falha ao carregar inventário:", err);
        } finally {
            UI.toggleLoading(false);
        }
    },

    render() {
        const searchInput = document.getElementById('inventory-search');
        const searchTerm = searchInput ? searchInput.value.toLowerCase() : "";

        let filtered = this.currentItems;
        if (searchTerm) {
            filtered = this.currentItems.filter(item =>
                (item.product?.name?.toLowerCase().includes(searchTerm)) ||
                (item.sku?.toLowerCase().includes(searchTerm))
            );
        }

        UI.renderInventoryTable(filtered);
    },

    async handleUpdate(id, change) {
        const success = await Api.updateStock(id, change);
        if (success) {
            await this.load();
        } else {
            alert("Erro ao atualizar estoque.");
        }
    }
};