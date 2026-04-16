(function () {
    'use strict';

    var CAROUSEL_INTERVAL = 5000;
    var ANIM_MS = 600;
    var FALLBACK_POLL = 30000;
    var WARNING_MINUTES = 30;
    var CRITICAL_MINUTES = 60;
    var SHOW_CLOCK = true;
    var ENABLE_SOUND = false;
    var AREA_ID = null;
    var USER_SECTOR_IDS = [];
    var ALL_AREAS = [];
    var requests = [];
    var sortedCards = [];
    var visibleStart = 0;
    var perPage = 6;
    var carouselTimer = null;
    var animating = false;
    var signalRConnected = false;

    var board = document.getElementById('andonBoard');
    var clock = document.getElementById('andonClock');
    var emptyState = document.getElementById('emptyState');
    var indicator = document.getElementById('pageIndicator');

    function esc(str) {
        var d = document.createElement('div');
        d.textContent = str || '';
        return d.innerHTML;
    }

    function timeSince(iso) {
        if (!iso) return '--';
        var s = Math.max(0, Math.floor((new Date() - new Date(iso)) / 1000));
        var d = Math.floor(s / 86400), h = Math.floor((s % 86400) / 3600),
            m = Math.floor((s % 3600) / 60), sec = s % 60;
        var p = function (n) { return n < 10 ? '0' + n : '' + n; };
        if (d > 0) return d + 'd ' + p(h) + ':' + p(m) + ':' + p(sec);
        if (h > 0) return p(h) + ':' + p(m) + ':' + p(sec);
        return p(m) + ':' + p(sec);
    }

    function urgencyCls(iso) {
        if (!iso) return '';
        var m = Math.floor((new Date() - new Date(iso)) / 60000);
        if (m >= CRITICAL_MINUTES) return 'urgency-critical';
        if (m >= WARNING_MINUTES) return 'urgency-warning';
        return '';
    }

    function updateClock() {
        clock.textContent = new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    }

    function makeCard(r) {
        var el = document.createElement('div');
        el.className = 'andon-card ' + urgencyCls(r.createdAt);
        el.innerHTML =
            '<div class="andon-card-header" style="background-color:' + esc(r.sectorColor || '#666') + '">' +
                '<span class="andon-code">' + esc(r.sectorName) + '</span>' +
            '</div>' +
            '<div class="andon-card-body">' +
                '<div class="andon-info-row">' +
                    '<div class="andon-info-item"><span class="andon-info-label">Recurso</span><span class="andon-info-value">' + esc(r.areaName) + '</span></div>' +
                '</div>' +
            '</div>' +
            '<div class="andon-card-footer">' +
                '<span class="andon-time"><span class="andon-time-value" data-created="' + esc(r.createdAt) + '">' + timeSince(r.createdAt) + '</span></span>' +
            '</div>';
        return el;
    }

    function calcPerPage() {
        var c = document.querySelector('.andon-board-container');
        if (!c) return 6;
        var cH = c.clientHeight, cW = c.clientWidth, minW = 420, gap = 8;
        var cols = Math.max(1, Math.floor((cW + gap) / (minW + gap)));
        var t = makeCard({ code: 'X', sectorName: 'X', sectorColor: '#999', areaName: 'X', createdAt: new Date().toISOString(), status: 0 });
        t.style.cssText = 'visibility:hidden;position:absolute';
        board.appendChild(t);
        var cardH = t.offsetHeight + gap;
        board.removeChild(t);
        if (cardH < 10) cardH = 200;
        var rows = Math.max(1, Math.floor((cH + gap) / cardH));
        return cols * rows;
    }

    function getWindow() {
        var items = [];
        for (var i = 0; i < perPage && i < sortedCards.length; i++) {
            items.push(sortedCards[(visibleStart + i) % sortedCards.length]);
        }
        return items;
    }

    function renderFull() {
        if (sortedCards.length === 0) {
            board.style.display = 'none';
            emptyState.style.display = '';
            indicator.classList.add('hidden');
            return;
        }
        board.style.display = '';
        emptyState.style.display = 'none';
        perPage = calcPerPage();
        if (visibleStart >= sortedCards.length) visibleStart = 0;
        board.innerHTML = '';
        getWindow().forEach(function (r) { board.appendChild(makeCard(r)); });
        updateDots();
    }

    // ---- Card-by-card FLIP slide ----
    function advanceOne() {
        if (sortedCards.length <= perPage || animating) return;
        animating = true;

        var cards = Array.prototype.slice.call(board.children);
        if (cards.length < 2) { animating = false; return; }

        // 1) Record current position of every card
        var rects = cards.map(function (c) { return c.getBoundingClientRect(); });

        // 2) Calculate target: each card[i] moves to card[i-1]'s position
        //    card[0] slides off-screen to the left
        var anims = [];

        for (var i = 0; i < cards.length; i++) {
            var from = rects[i];

            if (i === 0) {
                // First card: slide off-screen left (move it one card-width further left)
                var dx = -(from.width + 60);
                anims.push(cards[i].animate([
                    { transform: 'translate(0px, 0px)', opacity: 1 },
                    { transform: 'translate(' + dx + 'px, 0px)', opacity: 0 }
                ], { duration: ANIM_MS, easing: 'ease-in-out', fill: 'forwards' }));
            } else {
                // Other cards: slide to previous card's position
                var to = rects[i - 1];
                var ddx = to.left - from.left;
                var ddy = to.top - from.top;
                anims.push(cards[i].animate([
                    { transform: 'translate(0px, 0px)' },
                    { transform: 'translate(' + ddx + 'px, ' + ddy + 'px)' }
                ], { duration: ANIM_MS, easing: 'ease-in-out', fill: 'forwards' }));
            }
        }

        // 3) After all animations finish, rebuild
        setTimeout(function () {
            // Cancel all animations
            anims.forEach(function (a) { a.cancel(); });

            // Remove first card
            var first = board.children[0];
            if (first && first.parentNode) first.parentNode.removeChild(first);

            // Advance window
            visibleStart = (visibleStart + 1) % sortedCards.length;

            // Add new card at the end
            var newIdx = (visibleStart + perPage - 1) % sortedCards.length;
            var newEl = makeCard(sortedCards[newIdx]);
            board.appendChild(newEl);

            // Get the new card's final position to animate it in
            var newRect = newEl.getBoundingClientRect();
            newEl.animate([
                { transform: 'translate(' + (newRect.width + 60) + 'px, 0px)', opacity: 0 },
                { transform: 'translate(0px, 0px)', opacity: 1 }
            ], { duration: ANIM_MS, easing: 'ease-out', fill: 'forwards' });

            updateDots();

            setTimeout(function () {
                animating = false;
            }, ANIM_MS + 50);
        }, ANIM_MS + 50);
    }

    function updateDots() {
        var need = sortedCards.length > perPage;
        if (!need) { indicator.classList.add('hidden'); return; }
        indicator.classList.remove('hidden');
        var html = '';
        for (var i = 0; i < sortedCards.length; i++) {
            var vis = false;
            for (var v = 0; v < perPage && v < sortedCards.length; v++) {
                if ((visibleStart + v) % sortedCards.length === i) { vis = true; break; }
            }
            html += '<span class="andon-page-dot' + (vis ? ' active' : '') + '"></span>';
        }
        indicator.innerHTML = html;
    }

    function sortReqs() {
        sortedCards = requests.slice().sort(function (a, b) {
            var p = { 2: 0, 0: 1, 1: 2 };
            var pa = p[a.status] !== undefined ? p[a.status] : 9;
            var pb = p[b.status] !== undefined ? p[b.status] : 9;
            if (pa !== pb) return pa - pb;
            return new Date(a.createdAt) - new Date(b.createdAt);
        });
    }

    async function load() {
        try {
            var r = await fetch('/bff/help-requests');
            if (!r.ok) return;
            var all = await r.json();
            var active = all.filter(function (x) { return x.status === 0; });
            requests = filterByConfig(active);
        } catch (e) { }
        sortReqs();
        if (!animating) renderFull();
    }

    function tickTimers() {
        document.querySelectorAll('.andon-time-value').forEach(function (el) {
            var iso = el.getAttribute('data-created');
            if (iso) el.textContent = timeSince(iso);
        });
        // Update urgency classes on cards
        document.querySelectorAll('.andon-card').forEach(function (card) {
            var el = card.querySelector('.andon-time-value');
            if (!el) return;
            var iso = el.getAttribute('data-created');
            card.classList.remove('urgency-warning', 'urgency-critical');
            var cls = urgencyCls(iso);
            if (cls) card.classList.add(cls);
        });
    }

    function startCarousel() {
        if (carouselTimer) clearInterval(carouselTimer);
        carouselTimer = setInterval(advanceOne, CAROUSEL_INTERVAL);
    }

    // ---- SignalR ----
    async function connectSignalR() {
        if (typeof signalR === 'undefined') return;
        var conn = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/help-requests')
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .build();
        conn.on('HelpRequestsChanged', load);
        conn.onreconnected(function () { signalRConnected = true; load(); });
        conn.onclose(function () { signalRConnected = false; });
        try {
            await conn.start();
            signalRConnected = true;
            try {
                var me = await (await fetch('/bff/me')).json();
                if (me && me.tenantId) await conn.invoke('JoinTenant', me.tenantId);
            } catch (e) { }
        } catch (e) { signalRConnected = false; }
    }

    // ---- Init ----
    updateClock();
    setInterval(updateClock, 1000);
    setInterval(tickTimers, 1000);

    // Get all descendant area IDs for a given areaId
    function getAreaAndDescendants(areaId) {
        if (!areaId) return [];
        var ids = [areaId];
        var queue = [areaId];
        while (queue.length > 0) {
            var current = queue.shift();
            ALL_AREAS.forEach(function (a) {
                if (a.parentId && a.parentId === current) {
                    ids.push(a.id);
                    queue.push(a.id);
                }
            });
        }
        return ids;
    }

    function filterByConfig(items) {
        var result = items;

        // Filter by user's sectors
        if (USER_SECTOR_IDS.length > 0) {
            result = result.filter(function (r) {
                return USER_SECTOR_IDS.indexOf(r.sectorId) >= 0;
            });
        }

        // Filter by area (selected level + all sub-levels)
        if (AREA_ID && ALL_AREAS.length > 0) {
            var allowedAreaIds = getAreaAndDescendants(AREA_ID);
            result = result.filter(function (r) {
                return r.areaId && allowedAreaIds.indexOf(r.areaId) >= 0;
            });
        }

        return result;
    }

    async function loadAndonSettings() {
        try {
            var r = await fetch('/bff/andon-user-settings');
            if (r.ok) {
                var data = await r.json();
                if (typeof data.warningMinutes === 'number' && data.warningMinutes > 0) WARNING_MINUTES = data.warningMinutes;
                if (typeof data.criticalMinutes === 'number' && data.criticalMinutes > 0) CRITICAL_MINUTES = data.criticalMinutes;
                if (typeof data.carouselIntervalSeconds === 'number' && data.carouselIntervalSeconds > 0) {
                    CAROUSEL_INTERVAL = data.carouselIntervalSeconds * 1000;
                }
                if (typeof data.showClock === 'boolean') {
                    SHOW_CLOCK = data.showClock;
                    var clockEl = document.getElementById('andonClock');
                    var dotEl = document.querySelector('.andon-live-dot');
                    if (clockEl) clockEl.style.display = SHOW_CLOCK ? '' : 'none';
                    if (dotEl) dotEl.style.display = SHOW_CLOCK ? '' : 'none';
                }
                if (typeof data.enableSound === 'boolean') {
                    ENABLE_SOUND = data.enableSound;
                }
                if (data.areaId) {
                    AREA_ID = data.areaId;
                }
            }
        } catch (e) { }

        // Load current user data to get sectors
        try {
            var me = await (await fetch('/bff/me')).json();
            if (me && me.id) {
                var userData = await (await fetch('/bff/users/' + me.id)).json();
                if (userData && userData.sectors && userData.sectors.length > 0) {
                    USER_SECTOR_IDS = userData.sectors.map(function (s) { return s.sectorId; });
                }
            }
        } catch (e) { }

        // Load areas for hierarchy resolution
        if (AREA_ID) {
            try {
                var areasResp = await fetch('/bff/areas');
                if (areasResp.ok) {
                    ALL_AREAS = await areasResp.json();
                }
            } catch (e) { }
        }
    }

    (async function () {
        await loadAndonSettings();
        await load();
        startCarousel();
        await connectSignalR();
        setInterval(function () { if (!signalRConnected) load(); }, FALLBACK_POLL);
    })();

    window.addEventListener('resize', function () {
        perPage = calcPerPage();
        if (!animating) renderFull();
    });
})();
