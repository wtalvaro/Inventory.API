/**
 * APP.JS - O MAESTRO DA SPA
 * Responsabilidade: Orquestrar módulos e expor funções globais para o HTML.
 */

import { Api } from './modules/api.js';
import { UI } from './modules/ui.js';
import { Inventory } from './modules/inventory.js';
import { Session } from './modules/session.js';
import { SalesCoach } from './modules/salesCoach.js';
import { Telemetry } from './modules/telemetry.js';

// --- 1. EXPOSIÇÃO PARA O ESCOPO GLOBAL (window) ---
// Necessário para que atributos 'onclick' e 'onchange' no HTML funcionem com módulos.
window.showSection = (sectionId) => {
    // 1. Esconde todas as secções
    document.querySelectorAll('main section').forEach(s => s.classList.add('hidden'));

    // 2. Mostra a secção selecionada
    const target = document.getElementById(`sec-${sectionId}`);
    if (target) {
        target.classList.remove('hidden');
    }

    // 3. Se for telemetria, carrega os dados automaticamente
    if (sectionId === 'telemetry') {
        Telemetry.load();
    }
};
window.loadInventory = () => Inventory.load();

window.handleStockUpdate = (id, change) => Inventory.handleUpdate(id, change);

window.logout = () => Api.logout();

/**
 * Ativa o Coach de Vendas a partir de um clique na tabela de inventário
 */
window.activateCoach = (productId, name, sku) => {
    UI.showSection('coach');
    SalesCoach.init(productId, name, sku);
};

/**
 * Função de busca (Pode ser ligada a um evento 'oninput' no campo de pesquisa)
 */
window.searchInventory = () => Inventory.render();

window.loadTelemetry = () => Telemetry.load();

// --- 2. INICIALIZAÇÃO DO SISTEMA ---

document.addEventListener('DOMContentLoaded', () => {
    // 1. Remove o loading
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.classList.add('hidden');
        overlay.style.display = 'none';
    }

    // 2. Tenta detectar a autenticação de forma robusta
    // Verificamos o ServerState e também se o elemento principal da App existe na página
    const isLogged = window.ServerState?.isAuthenticated === true || !!document.getElementById('main-app');

    if (isLogged) {
        console.log("RetailPro SPA: Dashboard detectado. Inicializando...");
        Session.init();
        Inventory.load();
        UI.showSection('inventory');
    } else {
        console.log("RetailPro SPA: Tela de autenticação detectada.");
    }

    if (ServerState.role === 'Coordenador') {
        Telemetry.load(); // Carrega os dados de rede automaticamente para o boss
    }

    // Filtro em tempo real na tabela de inventário
    const searchInput = document.getElementById('inventory-search');
    if (searchInput) {
        searchInput.addEventListener('input', () => Inventory.render());
    }
});

// Tratamento de erros globais de promessas (Opcional, mas recomendado)
window.addEventListener('unhandledrejection', event => {
    console.error('Erro de API não tratado:', event.reason);
});