/**
 * INVENTORY.JS - Lógica de Negócio e Gestão de Stock
 * Responsabilidade: Controlar o catálogo local, filtros e ajustes manuais.
 */
import { Api } from './api.js';
import { UI } from './ui.js';

export const Inventory = {
    currentItems: [],

    /**
     * Carrega os dados de inventário baseados na loja selecionada
     */
    async load() {
        UI.toggleLoading(true);

        // 1. Identifica a loja (Filtro global para Coordenador ou fixa para Vendedor)
        const filterEl = document.getElementById('store-filter');
        const sId = filterEl ? parseInt(filterEl.value) : (ServerState.storeId || 0);

        try {
            const data = await Api.getInventory(sId);
            this.currentItems = data || [];
            this.render();
        } catch (err) {
            console.error("Erro ao carregar inventário:", err);
            UI.notify("Não foi possível carregar o stock.", "error");
        } finally {
            UI.toggleLoading(false);
        }
    },

    /**
     * Filtra e renderiza a tabela de inventário
     */
    render() {
        const searchInput = document.getElementById('inventory-search');
        const searchTerm = searchInput ? searchInput.value.toLowerCase().trim() : "";

        let filtered = this.currentItems;

        if (searchTerm) {
            filtered = this.currentItems.filter(item =>
                (item.product?.name?.toLowerCase().includes(searchTerm)) ||
                (item.product?.sku?.toLowerCase().includes(searchTerm)) ||
                (item.sku?.toLowerCase().includes(searchTerm))
            );
        }

        UI.renderInventoryTable(filtered);
    },

    /**
     * Processa ajustes manuais de stock (+1 / -1)
     * @param {number} id - ID do registo no StoreInventory
     * @param {number} delta - A variação (ex: 1 para entrada, -1 para saída)
     */
    async handleUpdate(id, delta) {
        // 1. Localiza o item no estado local
        const item = this.currentItems.find(i => i.id === id);
        if (!item) return;

        // 2. Lógica ERP: Todo ajuste manual exige um motivo para o InventoryLog
        const action = delta > 0 ? "ENTRADA" : "SAÍDA";
        const reason = prompt(`Motivo da ${action} de stock:`, "Ajuste manual via Dashboard");

        // Se o usuário cancelar o prompt, interrompemos o ajuste
        if (reason === null) return;

        const newQuantity = (item.quantity || 0) + delta;

        // Validação simples: não permitir stock negativo via ajuste manual
        if (newQuantity < 0) {
            UI.notify("O stock não pode ser inferior a zero.", "warning");
            return;
        }

        UI.toggleLoading(true);
        try {
            // Chamada à API refatorada (newQuantity e reason)
            const success = await Api.updateStock(id, newQuantity, reason);

            if (success) {
                // Atualização Otimista (UX): Muda o valor na memória antes do reload
                item.quantity = newQuantity;
                this.render();
                UI.notify("Stock atualizado com sucesso!", "success");
            } else {
                UI.notify("Erro ao comunicar com o servidor.", "error");
            }
        } catch (err) {
            UI.notify("Falha na operação de stock.", "error");
        } finally {
            UI.toggleLoading(false);
        }
    }
};