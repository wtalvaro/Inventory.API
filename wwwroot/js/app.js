/**
 * APP.JS - VERSÃO EXECUTORA (Modificada)
 */

// 1. Navegação de Abas (Inalterado)
function showSection(sectionId) {
    const sections = ['sec-inventory', 'sec-coach', 'sec-telemetry'];
    sections.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.classList.add('hidden');
    });

    const targetId = sectionId.startsWith('sec-') ? sectionId : `sec-${sectionId}`;
    const target = document.getElementById(targetId);

    if (target) {
        target.classList.remove('hidden');
        if (sectionId.includes('inventory')) loadInventory();
    }
}

// 2. Relógio Digital (Atualizado para ler o alerta do Razor)
function initSessionTimer() {
    const timerDisplay = document.getElementById('session-timer-display');
    const clockContainer = document.getElementById('session-clock');

    if (!ServerState.isAuthenticated || !timerDisplay) return;

    let secondsRemaining = parseInt(ServerState.sessionDurationSeconds);
    if (isNaN(secondsRemaining)) secondsRemaining = 7200;

    const interval = setInterval(() => {
        if (secondsRemaining <= 0) {
            clearInterval(interval);
            logout();
            return;
        }
        secondsRemaining--;

        const hrs = Math.floor(secondsRemaining / 3600);
        const mins = Math.floor((secondsRemaining % 3600) / 60);
        const secs = secondsRemaining % 60;
        const f = (n) => n.toString().padStart(2, '0');

        timerDisplay.innerText = `${f(hrs)}:${f(mins)}:${f(secs)}`;

        // MODIFICAÇÃO: Alerta dinâmico vindo do C#
        if (secondsRemaining < ServerState.alertAtSeconds) {
            clockContainer?.classList.add('text-red-500', 'animate-pulse');
        }
    }, 1000);
}

// 3. Renderização da Tabela (Inalterado)
function renderTable(items) {
    const tbody = document.getElementById('inventory-table-body');
    if (!tbody) return;

    if (!items || items.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="p-8 text-center text-gray-400">Nenhum item encontrado.</td></tr>';
        return;
    }

    tbody.innerHTML = items.map(item => {
        const nomeLoja = item.store ? item.store.name : `Unid. ${item.storeId}`;
        const nomeProd = item.product ? item.product.name : 'Produto s/ Nome';
        const preco = item.localPrice || (item.product ? item.product.price : 0);

        return `
            <tr class="border-b hover:bg-gray-50 text-gray-700 text-sm transition-colors">
                <td class="p-4 font-bold text-gray-500 text-center">${nomeLoja}</td>
                <td class="p-4 font-medium text-gray-900">${nomeProd}</td>
                <td class="p-4 font-mono text-gray-400 text-xs">${item.sku || 'N/A'}</td>
                <td class="p-4 font-bold text-blue-600 text-center text-base">${item.quantity ?? 0}</td>
                <td class="p-4 text-center">R$ ${parseFloat(preco).toFixed(2)}</td>
                <td class="p-4">
                    <div class="flex gap-2 justify-center">
                        <button onclick="updateStock(${item.id}, 1)" 
                                class="bg-green-100 px-3 py-1 rounded hover:bg-green-200 text-green-700 font-bold transition-transform active:scale-95">+</button>
                        <button onclick="updateStock(${item.id}, -1)" 
                                class="bg-red-100 px-3 py-1 rounded hover:bg-red-200 text-red-700 font-bold transition-transform active:scale-95">-</button>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

// 4. Chamadas de API (Simplificado)
async function loadInventory() {
    let sId = ServerState.role === 'Coordenador'
        ? (document.getElementById('store-filter')?.value || 0)
        : ServerState.storeId;

    try {
        const res = await fetch(`/api/inventory/store/${sId}`);
        if (res.status === 401) return window.location.reload();
        const data = await res.json();
        renderTable(data);
    } catch (err) { console.error("Erro ao buscar inventário:", err); }
}

async function updateStock(id, change) {
    try {
        const res = await fetch(`/api/inventory/${id}/stock?change=${change}`, { method: 'PATCH' });
        if (res.ok) loadInventory();
    } catch (err) { console.error(err); }
}

async function logout() {
    await fetch('/api/auth/logout', { method: 'POST' });
    window.location.reload();
}

// 5. Inicialização (Simplificada: Removemos loadStoreFilter)
document.addEventListener('DOMContentLoaded', () => {
    if (ServerState.isAuthenticated) {
        initSessionTimer();
        loadInventory();
        showSection('inventory');
    }
    const overlay = document.getElementById('loading-overlay');
    if (overlay) overlay.style.display = 'none';
});