// Reinitializa o menu do template após navegação Blazor
window.initTemplateMenu = function () {
    // Re-add has-sub class to li elements with ul children
    $('.navigation').find('li').has('ul').addClass('has-sub');

    var menuType = $('body').data('menu');

    // Re-apply active parent classes for compact menu
    if (menuType === 'vertical-compact-menu' || menuType === 'horizontal-menu') {
        $(".main-menu-content").find('li.active').parents('li:not(.nav-item)').addClass('open');
        $(".main-menu-content").find('li.active').parents('li').addClass('active');
    }

    // Clean up any leftover popout submenus from previous navigation
    $(".main-menu-content").children("span.menu-title").remove();
    $(".main-menu-content").children("a.menu-title").remove();
    $(".main-menu-content").children("ul.menu-content").remove();

    // Completely unbind and rebind menu events
    if (typeof $.app !== 'undefined' && typeof $.app.nav !== 'undefined') {
        // Remove ALL menu-related events
        $(".navigation-main").off();
        $(".main-menu-content").off();
        $(".navigation-main li a").off();
        $("ul.menu-content").off();

        // Refresh cached references
        $.app.nav.container = $(".navigation-main");
        $.app.nav.navItem = $(".navigation-main").find("li").not(".navigation-category");

        // Rebind cleanly
        $.app.nav.bind_events();
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