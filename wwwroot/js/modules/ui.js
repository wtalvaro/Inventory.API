/**
 * UI.JS - Gerenciamento de Interface e Renderização
 */
export const UI = {
    // Gerenciamento de Seções (SPA)
    showSection(sectionId) {
        const sections = ['sec-inventory', 'sec-coach', 'sec-telemetry'];
        sections.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.classList.add('hidden');
        });

        const targetId = sectionId.startsWith('sec-') ? sectionId : `sec-${sectionId}`;
        const target = document.getElementById(targetId);

        if (target) {
            target.classList.remove('hidden');
        }

        // Remove destaque de todos os botões e poderia adicionar ao ativo aqui
        this.updateActiveNavLink(sectionId);
    },

    updateActiveNavLink(sectionId) {
        // Lógica opcional para destacar o botão clicado na sidebar
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('bg-blue-50', 'text-blue-600');
        });
    },

    // Renderização da Tabela de Inventário
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
        tbody.innerHTML = items.map(item => this.createTableRow(item)).join('');
    },

    // Helper para criar a linha (Mantém o código limpo)
    createTableRow(item) {
        const nomeLoja = item.store ? item.store.name : `Unid. ${item.storeId}`;
        const nomeProd = item.product ? item.product.name : 'Produto s/ Nome';
        const preco = item.localPrice || (item.product ? item.product.price : 0);
        const pSku = item.sku || 'N/A';

        return `
            <tr class="border-b hover:bg-gray-50 text-gray-700 text-sm transition-colors">
                <td class="p-4 font-bold text-gray-500 text-center">${nomeLoja}</td>
                <td class="p-4 font-medium text-gray-900">${nomeProd}</td>
                <td class="p-4 font-mono text-gray-400 text-xs">${pSku}</td>
                <td class="p-4 font-bold text-blue-600 text-center text-base">${item.quantity ?? 0}</td>
                <td class="p-4 text-center">R$ ${parseFloat(preco).toFixed(2)}</td>
                <td class="p-4">
                    <div class="flex gap-2 justify-center">
                        <button onclick="activateCoach(${item.productId}, '${nomeProd.replace(/'/g, "\\'")}', '${pSku}')" 
                                class="bg-purple-100 p-2 rounded hover:bg-purple-200 text-purple-700 transition-all shadow-sm" title="Abrir Coach">
                            <i class="fas fa-magic"></i>
                        </button>
                        <button onclick="handleStockUpdate(${item.id}, 1)" 
                                class="bg-green-100 px-3 py-1 rounded hover:bg-green-200 text-green-700 font-bold transition-transform active:scale-95">+</button>
                        <button onclick="handleStockUpdate(${item.id}, -1)" 
                                class="bg-red-100 px-3 py-1 rounded hover:bg-red-200 text-red-700 font-bold transition-transform active:scale-95">-</button>
                    </div>
                </td>
            </tr>
        `;
    },

    toggleLoading(show) {
        const overlay = document.getElementById('loading-overlay');
        if (overlay) {
            show ? overlay.classList.remove('hidden') : overlay.classList.add('hidden');
        }
    }
};