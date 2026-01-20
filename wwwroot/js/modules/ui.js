/**
 * UI.JS - Gestor de Interface e Renderização (Refatorado)
 * Responsabilidade: Manipulação do DOM, tabelas e estados visuais.
 */

export const UI = {

    // --- 1. CORE & NAVEGAÇÃO (SPA) ---

    /**
     * Alterna entre as seções da SPA ocultando as demais.
     */
    showSection(sectionId) {
        const sections = ['sec-inventory', 'sec-coach', 'sec-telemetry', 'sec-purchase'];

        sections.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.classList.add('hidden');
        });

        // Garante que o ID tenha o prefixo 'sec-'
        const targetId = sectionId.startsWith('sec-') ? sectionId : `sec-${sectionId}`;
        const target = document.getElementById(targetId);

        if (target) {
            target.classList.remove('hidden');
            this.updateActiveNavLink(sectionId);
        }
    },

    /**
     * Atualiza o estado visual dos links na Sidebar.
     */
    updateActiveNavLink(sectionId) {
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('bg-blue-50', 'text-blue-600', 'bg-orange-50', 'text-orange-600');
        });
        // Aqui você pode adicionar lógica para destacar o botão baseado no sectionId
    },

    /**
     * Exibe ou oculta o overlay de carregamento global.
     */
    toggleLoading(show) {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            show ? overlay.classList.remove('hidden') : overlay.classList.add('hidden');
        }
    },

    /**
     * Notificações simples via Toast (Opcional, mas recomendado para ERP)
     */
    notify(message, type = 'success') {
        // Implementação futura ou usar alert simples por enquanto
        console.log(`[${type.toUpperCase()}] ${message}`);
        if (type === 'error') alert(message);
    },

    // --- 2. INVENTÁRIO ---

    /**
     * Renderiza a grade de produtos da loja selecionada.
     */
    renderInventoryTable(items) {
        const tbody = document.getElementById('inventory-table-body');
        const emptyState = document.getElementById('inventory-empty-state');

        if (!tbody) return;

        if (!items || items.length === 0) {
            tbody.innerHTML = '';
            emptyState?.classList.remove('hidden');
            return;
        }

        emptyState?.classList.add('hidden');
        tbody.innerHTML = items.map(item => this._createInventoryRow(item)).join('');
    },

    /**
     * Helper privado para criar a linha de inventário.
     */
    _createInventoryRow(item) {
        const nomeLoja = item.store?.name || `Unid. ${item.storeId}`;
        const nomeProd = item.product?.name || 'Produto s/ Nome';
        const preco = item.localPrice || (item.product?.price || 0);
        const pSku = item.sku || 'N/A';

        return `
            <tr class="border-b hover:bg-gray-50 text-gray-700 text-sm transition-colors">
                <td class="p-4 font-bold text-gray-400 text-center">${nomeLoja}</td>
                <td class="p-4">
                    <div class="font-medium text-gray-900">${nomeProd}</div>
                    <div class="text-[10px] text-gray-400 font-mono">${pSku}</div>
                </td>
                <td class="p-4 font-mono text-gray-400 text-xs text-center">${pSku}</td>
                <td class="p-4 font-black text-blue-600 text-center text-lg">${item.quantity ?? 0}</td>
                <td class="p-4 text-center font-semibold">R$ ${parseFloat(preco).toFixed(2)}</td>
                <td class="p-4 text-right">
                    <div class="flex gap-2 justify-end">
                        <button onclick="activateCoach(${item.productId}, '${nomeProd.replace(/'/g, "\\'")}', '${pSku}')" 
                                class="bg-purple-100 p-2.5 rounded-xl hover:bg-purple-600 hover:text-white text-purple-700 transition-all shadow-sm">
                            <i class="fas fa-magic"></i>
                        </button>
                        <button onclick="handleStockUpdate(${item.id}, 1)" 
                                class="bg-green-100 px-4 py-2 rounded-xl hover:bg-green-600 hover:text-white text-green-700 font-bold transition-all">+</button>
                        <button onclick="handleStockUpdate(${item.id}, -1)" 
                                class="bg-red-100 px-4 py-2 rounded-xl hover:bg-red-600 hover:text-white text-red-700 font-bold transition-all">-</button>
                    </div>
                </td>
            </tr>
        `;
    },

    // --- 3. ORDENS DE COMPRA (PURCHASE ORDERS) ---

    /**
     * Renderiza a lista principal de Ordens de Compra.
     */
    renderPurchaseOrders(orders) {
        const tbody = document.getElementById('purchase-orders-list');
        if (!tbody) return;

        if (!orders || orders.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="p-10 text-center text-gray-400 italic">Nenhuma ordem encontrada.</td></tr>';
            return;
        }

        tbody.innerHTML = orders.map(order => `
            <tr class="hover:bg-gray-50 border-b border-gray-50 transition-colors text-sm">
                <td class="p-4 font-mono text-[10px] text-gray-400">
                    #${order.id} <br> 
                    <span class="text-gray-600">${new Date(order.createdAt).toLocaleDateString()}</span>
                </td>
                <td class="p-4 font-bold text-gray-700">${order.supplier?.name || 'Fornecedor #' + order.supplierId}</td>
                <td class="p-4 text-gray-600"><i class="fas fa-store text-[10px] mr-1"></i> ${order.store?.name || 'Loja ' + order.storeId}</td>
                <td class="p-4 font-black text-blue-600">R$ ${order.totalCost.toFixed(2)}</td>
                <td class="p-4">
                    <span class="px-2 py-1 rounded-lg text-[10px] font-black uppercase ${this.getStatusColor(order.status)}">
                        ${order.status}
                    </span>
                </td>
                <td class="p-4 text-right">
                    ${order.status === 'Draft' ?
                `<button onclick="Purchase.manageItems(${order.id})" class="bg-orange-100 text-orange-600 hover:bg-orange-600 hover:text-white p-2.5 rounded-xl transition-all">
                            <i class="fas fa-edit"></i>
                        </button>` :
                `<button onclick="Purchase.manageItems(${order.id})" class="bg-gray-100 text-gray-500 hover:bg-blue-600 hover:text-white p-2.5 rounded-xl transition-all">
                            <i class="fas fa-eye"></i>
                        </button>`
            }
                </td>
            </tr>
        `).join('');
    },

    /**
     * Renderiza os itens dentro do modal de gerenciamento de OC.
     */
    renderPurchaseOrderItems(order) {
        const tbody = document.getElementById('po-items-table');
        const totalDisplay = document.getElementById('po-total-display');
        const poIdDisplay = document.getElementById('manage-po-id');

        if (!tbody) return;

        poIdDisplay.textContent = order.id;
        totalDisplay.textContent = `R$ ${order.totalCost.toFixed(2)}`;

        tbody.innerHTML = order.items.map(item => `
            <tr class="hover:bg-gray-50 transition-colors border-b border-gray-50">
                <td class="p-4">
                    <div class="font-bold text-gray-800">${item.product?.name || 'Produto'}</div>
                    <div class="text-[10px] text-gray-400 font-mono">${item.product?.sku || 'N/A'}</div>
                </td>
                <td class="p-4 text-center font-bold">${item.quantityOrdered}</td>
                <td class="p-4 text-center">R$ ${item.unitCost.toFixed(2)}</td>
                <td class="p-4 text-center font-black text-blue-600">R$ ${(item.quantityOrdered * item.unitCost).toFixed(2)}</td>
                <td class="p-4 text-right">
                    ${order.status === 'Draft' ?
                `<button onclick="Purchase.removeItem(${item.id})" class="text-red-400 hover:text-red-600 p-2"><i class="fas fa-trash"></i></button>`
                : '<i class="fas fa-lock text-gray-300"></i>'}
                </td>
            </tr>
        `).join('');
    },

    /**
     * Cores de Status baseadas no sistema de design ERP.
     */
    getStatusColor(status) {
        const colors = {
            'Draft': 'bg-gray-100 text-gray-500',
            'Pending': 'bg-blue-100 text-blue-600',
            'Received': 'bg-green-100 text-green-600',
            'Cancelled': 'bg-red-100 text-red-600'
        };
        return colors[status] || 'bg-gray-100 text-gray-400';
    },

    // --- 4. MODAIS & FORMULÁRIOS ---

    openModal(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.remove('hidden');
            modal.classList.add('flex');
            document.body.classList.add('overflow-hidden'); // Trava o scroll do fundo
        }
    },

    closeModal(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('hidden');
            modal.classList.remove('flex');
            document.body.classList.remove('overflow-hidden');
        }
    },

    /**
     * Popula os campos de seleção do modal de compra.
     */
    setupOrderModal(suppliers, stores) {
        const supplierSelect = document.getElementById('po-supplier-id');
        const storeSelect = document.getElementById('po-store-id');

        if (supplierSelect) {
            supplierSelect.innerHTML = '<option value="">Selecione um Fornecedor...</option>' +
                suppliers.map(s => `<option value="${s.id}">${s.name}</option>`).join('');
        }

        if (storeSelect) {
            // Se stores não vier da API, usa o do estado do servidor
            const options = stores && stores.length > 0
                ? stores.map(s => `<option value="${s.id}">${s.name}</option>`).join('')
                : `<option value="${ServerState.storeId || 0}">Loja Principal</option>`;

            storeSelect.innerHTML = options;
        }
    }
};