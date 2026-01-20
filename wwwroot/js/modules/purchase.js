/**
 * PURCHASE.JS - Módulo de Gestão de Suprimentos
 * Responsabilidade: Orquestrar o fluxo de rascunho, itens e recebimento de mercadorias.
 */
import { Api } from './api.js';
import { UI } from './ui.js';
import { Inventory } from './inventory.js';

export const Purchase = {
    // Estado interno do módulo
    currentOrder: null,
    selectedProduct: null,

    /**
     * Carrega a listagem principal de ordens de compra
     */
    async load() {
        UI.toggleLoading(true);
        try {
            const orders = await Api.getPurchaseOrders();
            UI.renderPurchaseOrders(orders);
        } catch (error) {
            console.error("Falha ao carregar ordens:", error);
            UI.notify("Erro ao carregar lista de compras.", "error");
        } finally {
            UI.toggleLoading(false);
        }
    },

    /**
     * Prepara e abre o modal para iniciar uma nova OC
     */
    async openNewOrderModal() {
        UI.toggleLoading(true);
        try {
            const suppliers = await Api.getSuppliers();
            const stores = await Api.getStores();

            UI.setupOrderModal(suppliers, stores);
            UI.openModal('modal-new-order');
        } catch (error) {
            UI.notify("Erro ao carregar dados de fornecedores.", "error");
        } finally {
            UI.toggleLoading(false);
        }
    },

    /**
     * Cria o rascunho inicial no banco de dados
     */
    async submitNewOrder() {
        const supplierId = document.getElementById('po-supplier-id')?.value;
        const storeId = document.getElementById('po-store-id')?.value;

        if (!supplierId || !storeId) return UI.notify("Selecione fornecedor e loja.", "warning");

        const data = {
            supplierId: parseInt(supplierId),
            storeId: parseInt(storeId),
            status: 'Draft'
        };

        UI.toggleLoading(true);
        const order = await Api.createPurchaseOrder(data);
        UI.toggleLoading(false);

        if (order) {
            UI.closeModal('modal-new-order');
            await this.manageItems(order.id);
            await this.load(); // Atualiza a tabela de fundo
        }
    },

    /**
     * Abre o modal de gerenciamento de itens (Edição ou Visualização)
     */
    async manageItems(orderId) {
        UI.toggleLoading(true);
        try {
            const order = await Api.getPurchaseOrderById(orderId);
            if (!order) return;

            this.currentOrder = order;

            // Lógica de UI: Se não for rascunho, esconde controles de edição
            const isEditable = order.status === 'Draft' || order.status === 0;
            this.toggleEditMode(isEditable);

            UI.renderPurchaseOrderItems(order);
            UI.openModal('modal-manage-items');
        } finally {
            UI.toggleLoading(false);
        }
    },

    /**
     * Alterna a visibilidade dos campos de edição dependendo do status da OC
     */
    toggleEditMode(show) {
        const searchBar = document.getElementById('po-search-product')?.closest('.bg-orange-50');
        const finalizeBtn = document.querySelector('button[onclick="Purchase.finalizeDraft()"]');

        if (show) {
            searchBar?.classList.remove('hidden');
            finalizeBtn?.classList.remove('hidden');
        } else {
            searchBar?.classList.add('hidden');
            finalizeBtn?.classList.add('hidden');
        }
    },

    /**
     * Autocomplete de busca de produtos
     */
    async searchProducts(query) {
        const resultsEl = document.getElementById('po-search-results');
        if (!query || query.length < 2) {
            resultsEl.classList.add('hidden');
            return;
        }

        const products = await Api.searchProducts(query);
        if (!products || products.length === 0) {
            resultsEl.innerHTML = '<div class="p-3 text-xs text-gray-400">Nenhum produto encontrado.</div>';
        } else {
            resultsEl.innerHTML = products.map(p => `
                <div onclick="Purchase.selectProductForOrder(${p.id}, '${p.name.replace(/'/g, "\\'")}', '${p.sku}')"
                     class="p-3 hover:bg-orange-50 cursor-pointer border-b border-gray-50 flex justify-between items-center group">
                    <div>
                        <div class="font-bold text-gray-700 text-sm group-hover:text-orange-700">${p.name}</div>
                        <div class="text-[10px] text-gray-400">Preço Base: R$ ${p.basePrice?.toFixed(2) || '0.00'}</div>
                    </div>
                    <span class="text-[10px] font-mono text-gray-400 bg-gray-100 px-2 py-1 rounded">${p.sku}</span>
                </div>
            `).join('');
        }
        resultsEl.classList.remove('hidden');
    },

    selectProductForOrder(id, name, sku) {
        this.selectedProduct = { id, name, sku };
        document.getElementById('po-search-product').value = `${name} (${sku})`;
        document.getElementById('po-search-results').classList.add('hidden');
        document.getElementById('btn-add-po-item').disabled = false;
        document.getElementById('po-add-qty').focus();
    },

    /**
     * Adiciona o item selecionado à Ordem (Lógica Delta de Custo)
     */
    async addItem() {
        if (!this.selectedProduct || !this.currentOrder) return;

        const qtyEl = document.getElementById('po-add-qty');
        const costEl = document.getElementById('po-add-cost');

        const qty = parseInt(qtyEl.value);
        const cost = parseFloat(costEl.value);

        if (!qty || qty <= 0 || !cost || cost <= 0) {
            return UI.notify("Informe quantidade e custo válidos.", "warning");
        }

        const itemData = {
            purchaseOrderId: this.currentOrder.id,
            productId: this.selectedProduct.id,
            quantityOrdered: qty,
            unitCost: cost
        };

        UI.toggleLoading(true);
        const result = await Api.addOrderItem(itemData);
        UI.toggleLoading(false);

        if (result) {
            await this.refreshItems();
            this.resetAddItemFields();
            qtyEl.focus();
        }
    },

    async removeItem(itemId) {
        if (!confirm("Remover este produto da ordem de compra?")) return;

        UI.toggleLoading(true);
        const success = await Api.deleteOrderItem(itemId);
        UI.toggleLoading(false);

        if (success) {
            await this.refreshItems();
        }
    },

    /**
     * Sincroniza os itens do modal e o total da ordem
     */
    async refreshItems() {
        if (!this.currentOrder) return;
        const updatedOrder = await Api.getPurchaseOrderById(this.currentOrder.id);
        this.currentOrder = updatedOrder;
        UI.renderPurchaseOrderItems(updatedOrder);
        this.load(); // Atualiza valor na tabela de fundo
    },

    /**
     * Muda status para Pendente (Bloqueia edição e aguarda chegada)
     */
    async finalizeDraft() {
        if (!this.currentOrder || this.currentOrder.items.length === 0) {
            return UI.notify("Adicione pelo menos um item.", "warning");
        }

        if (!confirm("Finalizar rascunho? A ordem será enviada para o fornecedor.")) return;

        UI.toggleLoading(true);
        // Nota: O backend deve tratar a mudança de status
        const success = await Api.receivePurchaseOrder(this.currentOrder.id); // Reuso de método para mudar status ou criar um PATCH
        UI.toggleLoading(false);

        if (success) {
            UI.closeModal('modal-manage-items');
            UI.notify("Ordem de Compra finalizada!", "success");
            this.load();
        }
    },

    /**
     * Aciona o Recebimento Efetivo (Gera Delta de Estoque e Lotes FIFO)
     */
    async receiveOrder(orderId) {
        if (!confirm("Confirmar recebimento físico? Isso atualizará o estoque imediatamente.")) return;

        UI.toggleLoading(true);
        const success = await Api.receivePurchaseOrder(orderId);
        UI.toggleLoading(false);

        if (success) {
            UI.notify("Estoque atualizado e lotes gerados!", "success");
            this.load();
            Inventory.load(); // Atualiza a aba de inventário se estiver aberta
        }
    },

    resetAddItemFields() {
        this.selectedProduct = null;
        document.getElementById('po-search-product').value = '';
        document.getElementById('po-add-qty').value = '';
        document.getElementById('po-add-cost').value = '';
        document.getElementById('btn-add-po-item').disabled = true;
    }
};