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

    // Refresh $.app.nav references and rebind events so the compact-menu
    // hover flyout works reliably after Blazor enhanced navigation.
    if (typeof $.app !== 'undefined' && typeof $.app.nav !== 'undefined') {
        // Unbind ALL stale delegated events that bind_events will re-add
        $(".navigation-main").off("mouseenter.app.menu mouseleave.app.menu active.app.menu deactive.app.menu open.app.menu close.app.menu click.app.menu");
        $(".main-menu-content").off("mouseleave");
        $(".navigation-main li.has-sub > a").off("click");
        $("ul.menu-content").off("click");

        // Refresh the cached container / navItem references
        $.app.nav.container = $(".navigation-main");
        $.app.nav.navItem = $(".navigation-main").find("li").not(".navigation-category");

        // Rebind events cleanly
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