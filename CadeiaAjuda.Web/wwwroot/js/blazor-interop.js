// Reinitializa o menu do template após navegação Blazor
// Reinitializa o menu do template após navegação Blazor
window.initTemplateMenu = function () {
    if (typeof $.app !== 'undefined' && typeof $.app.menu !== 'undefined') {
        $.app.menu.init();
    }
};

// Fullscreen toggle
function toggleFullScreen() {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen();
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
}