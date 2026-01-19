import { Api } from './api.js';

export const Session = {
    interval: null,
    secondsRemaining: 0,

    // CONFIGURAÇÃO: Tempo de inatividade (Ex: 15 minutos = 900 segundos)
    MAX_INACTIVITY: 900,
    inactivitySeconds: 0,

    init() {
        if (!ServerState.isAuthenticated) return;

        // Recupera o tempo vindo do Razor (GetRemainingSeconds)
        this.secondsRemaining = parseInt(ServerState.sessionDurationSeconds) || 7200;
        this.inactivitySeconds = 0;

        this.startTimer();
        this.setupInactivityListeners();
    },

    startTimer() {
        const timerDisplay = document.getElementById('session-timer-display');
        const clockContainer = document.getElementById('session-clock');

        if (!timerDisplay) return;

        this.interval = setInterval(() => {
            // 1. Verificação de Inatividade Local
            this.inactivitySeconds++;
            if (this.inactivitySeconds >= this.MAX_INACTIVITY) {
                this.handleSessionExpired("Sessão encerrada por inatividade prolongada.");
                return;
            }

            // 2. Verificação de Tempo Total da Sessão (Servidor)
            if (this.secondsRemaining <= 0) {
                this.handleSessionExpired("Sessão expirada. Por favor, faça login novamente.");
                return;
            }

            this.secondsRemaining--;

            // Atualização Visual
            timerDisplay.innerText = this.formatTime(this.secondsRemaining);

            if (this.secondsRemaining < ServerState.alertAtSeconds) {
                clockContainer?.classList.add('text-red-500', 'animate-pulse');
            }
        }, 1000);
    },

    setupInactivityListeners() {
        // Eventos que indicam que o usuário ainda está presente
        const events = ['mousedown', 'mousemove', 'keydown', 'scroll', 'touchstart'];

        const resetAction = () => {
            this.inactivitySeconds = 0; // Zera o contador sempre que houver interação
        };

        events.forEach(name => {
            document.addEventListener(name, resetAction, { passive: true });
        });
    },

    formatTime(totalSeconds) {
        const hrs = Math.floor(totalSeconds / 3600);
        const mins = Math.floor((totalSeconds % 3600) / 60);
        const secs = totalSeconds % 60;
        const pad = (n) => n.toString().padStart(2, '0');
        return `${pad(hrs)}:${pad(mins)}:${pad(secs)}`;
    },

    async handleSessionExpired(msg) {
        this.stopTimer();
        if (msg) alert(msg);
        await Api.logout();
    },

    stopTimer() {
        if (this.interval) clearInterval(this.interval);
    }
};