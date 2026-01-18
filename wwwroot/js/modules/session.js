/**
 * SESSION.JS - Gerenciamento de Sessão e Temporizadores
 */
import { Api } from './api.js';

export const Session = {
    interval: null,
    secondsRemaining: 0,

    init() {
        if (!ServerState.isAuthenticated) return;

        // Recupera o tempo vindo do Razor (GetRemainingSeconds)
        this.secondsRemaining = parseInt(ServerState.sessionDurationSeconds) || 7200;

        this.startTimer();
    },

    startTimer() {
        const timerDisplay = document.getElementById('session-timer-display');
        const clockContainer = document.getElementById('session-clock');

        if (!timerDisplay) return;

        this.interval = setInterval(() => {
            if (this.secondsRemaining <= 0) {
                this.handleSessionExpired();
                return;
            }

            this.secondsRemaining--;

            // Atualiza o texto no formato 00:00:00
            timerDisplay.innerText = this.formatTime(this.secondsRemaining);

            // Alerta visual de expiração (definido no ServerState)
            if (this.secondsRemaining < ServerState.alertAtSeconds) {
                clockContainer?.classList.add('text-red-500', 'animate-pulse');
            }
        }, 1000);
    },

    formatTime(totalSeconds) {
        const hrs = Math.floor(totalSeconds / 3600);
        const mins = Math.floor((totalSeconds % 3600) / 60);
        const secs = totalSeconds % 60;

        const pad = (n) => n.toString().padStart(2, '0');
        return `${pad(hrs)}:${pad(mins)}:${pad(secs)}`;
    },

    async handleSessionExpired() {
        this.stopTimer();
        alert("Sua sessão expirou por inatividade. Você será redirecionado.");
        await Api.logout();
    },

    stopTimer() {
        if (this.interval) clearInterval(this.interval);
    }
};