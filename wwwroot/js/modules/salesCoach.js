/**
 * SALESCOACH.JS - Lógica do Roteiro Dinâmico de Vendas
 */
import { Api } from './api.js';

export const SalesCoach = {
    timer: 0,
    interval: null,
    steps: [],
    isActive: false,

    // Configuração visual para cada tipo de passo (SalesStepType)
    config: {
        'Rapport': { badge: 'bg-blue-100 text-blue-700', border: 'border-blue-400' },
        'Sondagem': { badge: 'bg-yellow-100 text-yellow-700', border: 'border-yellow-400' },
        'Fechamento': { badge: 'bg-green-100 text-green-700', border: 'border-green-400' }
    },

    async init(productId, productName, productSku) {
        this.reset();

        // 1. Atualiza a UI com os dados do produto selecionado
        document.getElementById('focused-product-name').textContent = productName;
        document.getElementById('focused-product-sku').textContent = productSku;

        // 2. Busca a timeline da API
        this.steps = await Api.getSalesCoachTimeline(productId);

        // MOCK: Caso a API ainda não tenha dados, injetamos passos de teste
        if (this.steps.length === 0) {
            this.steps = [
                { second: 2, type: 'Rapport', message: 'Dê as boas-vindas e sorria! Pergunte se o cliente busca algo específico.' },
                { second: 15, type: 'Sondagem', message: 'Tente entender o perfil de uso: é para presente ou uso pessoal?' },
                { second: 35, type: 'Fechamento', message: 'Destaque o valor promocional e a garantia estendida.' }
            ];
        }

        this.start();
    },

    start() {
        this.isActive = true;
        document.getElementById('coach-placeholder')?.classList.add('hidden');
        document.getElementById('active-step-display')?.classList.remove('hidden');

        this.interval = setInterval(() => {
            this.timer++;
            this.updateUI();
        }, 1000);
    },

    updateUI() {
        // Formata o cronômetro
        const mins = Math.floor(this.timer / 60).toString().padStart(2, '0');
        const secs = (this.timer % 60).toString().padStart(2, '0');
        const timerEl = document.getElementById('coach-timer');
        if (timerEl) timerEl.textContent = `${mins}:${secs}`;

        // Barra de progresso (baseada em 1 minuto de ciclo)
        const progressEl = document.getElementById('coach-progress');
        if (progressEl) {
            const progress = Math.min((this.timer / 60) * 100, 100);
            progressEl.style.width = `${progress}%`;
        }

        // Procura se há uma mensagem para o segundo atual
        const step = this.steps.find(s => s.second === this.timer);
        if (step) this.renderStep(step);
    },

    renderStep(step) {
        const badge = document.getElementById('step-badge');
        const message = document.getElementById('step-message');
        const container = document.getElementById('coach-display-container');

        if (!badge || !message) return;

        const style = this.config[step.type] || this.config['Rapport'];

        // Atualiza conteúdo e estilo
        badge.textContent = step.type;
        badge.className = `inline-block px-3 py-1 rounded-md text-[10px] font-black uppercase mb-4 ${style.badge}`;
        message.textContent = step.message;

        if (container) {
            container.className = `min-h-[300px] bg-white rounded-3xl border-2 p-8 transition-all duration-500 ${style.border}`;
        }

        // Animação simples de entrada
        message.animate([
            { opacity: 0, transform: 'translateY(10px)' },
            { opacity: 1, transform: 'translateY(0)' }
        ], { duration: 500 });
    },

    reset() {
        if (this.interval) clearInterval(this.interval);
        this.timer = 0;
        this.isActive = false;
        const progressEl = document.getElementById('coach-progress');
        if (progressEl) progressEl.style.width = "0%";
    }
};