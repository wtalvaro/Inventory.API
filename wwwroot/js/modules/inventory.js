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
        // Busca o item atual na nossa lista local para saber a quantidade antes do ajuste
        const item = this.currentItems.find(i => i.id === id);
        if (!item) return;

        const success = await Api.updateStock(id, item.quantity, change);

        if (success) {
            // Atualiza a interface sem recarregar tudo (Performance)
            item.quantity += change;
            this.render();
        } else {
            alert("Não foi possível atualizar o estoque. Verifique suas permissões.");
        }
    }
};