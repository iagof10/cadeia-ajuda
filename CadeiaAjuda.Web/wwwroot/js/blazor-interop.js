// Reinitializa o menu do template após navegação Blazor
window.initTemplateMenu = function () {
    // Re-add has-sub class to li elements with ul children
    $('.navigation').find('li').has('ul').addClass('has-sub');

    // Re-apply active parent classes for compact menu
    var menuType = $('body').data('menu');
    if (menuType === 'vertical-compact-menu') {
        $(".main-menu-content").find('li.active').parents('li:not(.nav-item)').addClass('open');
        $(".main-menu-content").find('li.active').parents('li').addClass('active');
    }

    // Re-init menu if available
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