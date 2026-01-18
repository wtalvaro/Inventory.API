/**
 * TELEMETRY.JS - Lógica de Atualização do Painel
 */
import { Api } from './api.js';
import { UI } from './ui.js';

export const Telemetry = {
    async load() {
        UI.toggleLoading(true);
        console.log("Telemetry: A carregar dados...");

        try {
            const data = await Api.getTelemetryData();
            if (data) {
                this.render(data);
            }
        } catch (err) {
            console.error("Falha ao carregar telemetria:", err);
            // Opcional: Mostrar alerta visual de erro
        } finally {
            UI.toggleLoading(false);
            this.updateTimestamp();
        }
    },

    updateTimestamp() {
        const timeEl = document.getElementById('telemetry-last-update');
        if (timeEl) timeEl.textContent = new Date().toLocaleTimeString();
    },

    render(data) {
        // --- Atualização dos Cards de Resumo ---
        // Nota: Ajustei os IDs para baterem com o HTML abaixo
        this.safeSetContent('tel-total-sales', `R$ ${data.totalSales?.toFixed(2) || '0,00'}`);
        this.safeSetContent('tel-active-sessions', data.activeSessions || 0);
        this.safeSetContent('tel-stock-out', data.outOfStockCount || 0);
        this.safeSetContent('tel-avg-ticket', `R$ ${data.averageTicket?.toFixed(2) || '0,00'}`);

        // --- Renderização do Grid de Unidades ---
        const grid = document.getElementById('telemetry-grid');
        if (grid && data.storePerformances) {
            grid.innerHTML = data.storePerformances.map(store => `
                <div class="bg-gray-50 p-4 rounded-xl border border-gray-100 hover:shadow-md transition-shadow">
                    <p class="text-xs font-black text-gray-400 uppercase">${store.storeName}</p>
                    <div class="flex justify-between items-end mt-2">
                        <span class="text-xl font-bold">${store.stockLevel} un.</span>
                        <span class="text-${store.salesGrowth >= 0 ? 'green' : 'red'}-600 text-xs font-bold">
                            <i class="fas fa-caret-${store.salesGrowth >= 0 ? 'up' : 'down'}"></i> 
                            ${Math.abs(store.salesGrowth)}%
                        </span>
                    </div>
                </div>
            `).join('');
        }
    },

    // Função auxiliar para evitar erros se o ID não existir
    safeSetContent(id, value) {
        const el = document.getElementById(id);
        if (el) el.textContent = value;
    }
};

// Expõe a função para o botão HTML (onClick)
window.loadTelemetry = () => Telemetry.load();