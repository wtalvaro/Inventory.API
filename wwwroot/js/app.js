/**
 * APP.JS - O MAESTRO DA SPA (Refatorado)
 * Responsabilidade: Orquestrar módulos e expor funções globais para o HTML.
 */

import { Api } from './modules/api.js';
import { UI } from './modules/ui.js';
import { Inventory } from './modules/inventory.js';
import { Session } from './modules/session.js';
import { SalesCoach } from './modules/salesCoach.js';
import { Telemetry } from './modules/telemetry.js';
import { Purchase } from './modules/purchase.js'; // Novo módulo de compras

// --- 1. EXPOSIÇÃO PARA O ESCOPO GLOBAL (window) ---
// Necessário para manter a compatibilidade com 'onclick' e 'onchange' do HTML
window.showSection = (sectionId) => {
    // 1. Esconde todas as secções usando o padrão de ID 'sec-'
    document.querySelectorAll('main section').forEach(s => s.classList.add('hidden'));

    // 2. Mostra a secção selecionada
    const target = document.getElementById(`sec-${sectionId}`);
    if (target) {
        target.classList.remove('hidden');
    }

    // 3. Inicialização condicional por seção
    switch (sectionId) {
        case 'telemetry':
            Telemetry.load();
            break;
        case 'purchase':
            // Só carrega compras se for Admin ou Coordenador
            if (['Administrador', 'Coordenador'].includes(ServerState.role)) {
                Purchase.load();
            }
            break;
        case 'inventory':
            Inventory.load();
            break;
    }
};

// Atalhos globais para componentes específicos
window.UI = UI;
window.Inventory = Inventory;
window.Purchase = Purchase;
window.SalesCoach = SalesCoach;
window.Telemetry = Telemetry;

// Funções de ação rápida
window.loadInventory = () => Inventory.load();
window.handleStockUpdate = (id, change) => Inventory.handleUpdate(id, change);
window.logout = () => Api.logout();
window.searchInventory = () => Inventory.render();
window.loadTelemetry = () => Telemetry.load();

/**
 * Ativa o Coach de Vendas a partir da tabela de inventário
 */
window.activateCoach = (productId, name, sku) => {
    window.showSection('coach');
    SalesCoach.init(productId, name, sku);
};

// --- 2. INICIALIZAÇÃO DO SISTEMA ---

document.addEventListener('DOMContentLoaded', () => {
    // 1. Tratamento do Loading Overlay
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.classList.add('hidden');
        overlay.style.display = 'none';
    }

    // 2. Detecção de Autenticação e Estado Inicial
    const isLogged = window.ServerState?.isAuthenticated === true || !!document.getElementById('main-app');

    if (isLogged) {
        console.log("🚀 RetailPro SPA: Dashboard detectado. Inicializando...");

        Session.init();

        // Carregamento inicial baseado na seção padrão (Inventário)
        Inventory.load();
        window.showSection('inventory');

        // Automação para o Coordenador (Carregamento de fundo)
        if (ServerState.role === 'Coordenador') {
            Telemetry.load();
        }
    } else {
        console.log("🔑 RetailPro SPA: Tela de autenticação detectada.");
    }

    // 3. Event Listeners de UI
    const searchInput = document.getElementById('inventory-search');
    if (searchInput) {
        searchInput.addEventListener('input', () => Inventory.render());
    }
});

// Tratamento global de falhas de rede/API
window.addEventListener('unhandledrejection', event => {
    console.error('🔴 Erro de API não tratado:', event.reason);
});