(function () {
    'use strict';

    var dashData = null;

    function esc(str) {
        var d = document.createElement('div');
        d.textContent = str || '';
        return d.innerHTML;
    }

    function fmtDuration(minutes) {
        if (!minutes || minutes <= 0) return '\u2014';
        if (minutes < 60) return Math.round(minutes) + 'min';
        var h = Math.floor(minutes / 60);
        var m = Math.round(minutes % 60);
        return h + 'h' + (m > 0 ? ' ' + m + 'min' : '');
    }

    function fmtDate(iso) {
        if (!iso) return '\u2014';
        var d = new Date(iso);
        return d.toLocaleDateString('pt-BR') + ' ' + d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    }

    function statusClass(s) {
        switch (s) {
            case 0: return 'status-open';
            case 1: return 'status-inprogress';
            case 2: return 'status-escalated';
            case 3: return 'status-resolved';
            case 4: return 'status-closed';
            default: return '';
        }
    }

    function statusColor(s) {
        switch (s) {
            case 0: return '#ffc107';
            case 1: return '#1e9ff2';
            case 2: return '#f44336';
            case 3: return '#4caf50';
            case 4: return '#9e9e9e';
            default: return '#ccc';
        }
    }

    // ---- Load data ----
    async function loadDashboard() {
        try {
            var resp = await fetch('/bff/dashboard');
            if (!resp.ok) throw new Error('Erro');
            dashData = await resp.json();

            document.getElementById('dashLoading').style.display = 'none';
            document.getElementById('dashContent').style.display = '';
            document.getElementById('dashLastUpdate').textContent =
                'Atualizado em ' + new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

            renderKPIs();
            renderPeriodChart('week');
            renderStatusChart();
            renderSectorRanking();
            renderTypeRanking();
            renderRequesterRanking();
            renderPeakHours();
            renderMonthCompare();
            renderRecentTable();
            bindPeriodButtons();
        } catch (e) {
            document.getElementById('dashLoading').innerHTML =
                '<i class="la la-warning font-large-2 text-danger"></i><p class="text-danger mt-1">Erro ao carregar dashboard</p>';
        }
    }

    // ---- KPIs ----
    function renderKPIs() {
        var d = dashData;
        document.getElementById('kpiOpen').textContent = d.openCount;
        document.getElementById('kpiClosedToday').textContent = d.closedTodayCount;
        document.getElementById('kpiEscalated').textContent = d.escalatedCount;
        document.getElementById('kpiAvgTime').textContent = fmtDuration(d.avgCloseTimeMinutes);
        document.getElementById('kpiTotal').textContent = d.totalCount;
        document.getElementById('kpiUsers').textContent = d.activeUsersCount;
        document.getElementById('kpiOldest').textContent = d.openCount > 0 ? fmtDuration(d.oldestOpenMinutes) : '\u2014';

        // Month compare KPI
        var kpiMonth = document.getElementById('kpiMonthCompare');
        var kpiTrend = document.getElementById('kpiMonthTrend');
        var kpiIcon = document.getElementById('kpiMonthIcon');
        kpiMonth.textContent = d.currentMonthCount;

        var pct = d.monthOverMonthChangePercent;
        if (pct > 0) {
            kpiMonth.className = 'danger';
            kpiIcon.className = 'la la-arrow-up danger font-large-2 float-right';
            kpiTrend.innerHTML = '<span class="trend-up"><i class="la la-arrow-up"></i> +' + pct + '% vs mês anterior</span>';
        } else if (pct < 0) {
            kpiMonth.className = 'success';
            kpiIcon.className = 'la la-arrow-down success font-large-2 float-right';
            kpiTrend.innerHTML = '<span class="trend-down"><i class="la la-arrow-down"></i> ' + pct + '% vs mês anterior</span>';
        } else {
            kpiMonth.className = 'info';
            kpiIcon.className = 'la la-calendar info font-large-2 float-right';
            kpiTrend.innerHTML = '<span class="trend-neutral">Sem variação vs mês anterior</span>';
        }
    }

    // ---- Period Chart ----
    function renderPeriodChart(period) {
        var data;
        if (period === 'week') data = dashData.last7Days;
        else if (period === 'month') data = dashData.last4Weeks;
        else data = dashData.last6Months;

        var area = document.getElementById('periodChartArea');
        if (!data || data.length === 0) {
            area.innerHTML = '<span class="text-muted">Sem dados</span>';
            return;
        }

        var max = Math.max.apply(null, data.map(function (d) { return d.count; }));
        if (max === 0) max = 1;

        var html = '';
        data.forEach(function (item) {
            var pct = Math.max(6, Math.round((item.count / max) * 100));
            html += '<div class="dash-bar-row">' +
                '<span class="dash-bar-label">' + esc(item.label) + '</span>' +
                '<div class="dash-bar-track"><div class="dash-bar-fill" style="width:' + pct + '%;background:#1e9ff2;">' + item.count + '</div></div>' +
                '</div>';
        });
        area.innerHTML = html;
    }

    function bindPeriodButtons() {
        var btns = document.querySelectorAll('#periodBtnGroup button');
        btns.forEach(function (btn) {
            btn.addEventListener('click', function () {
                btns.forEach(function (b) { b.classList.remove('active'); });
                btn.classList.add('active');
                renderPeriodChart(btn.getAttribute('data-period'));
            });
        });
    }

    // ---- Status Chart ----
    function renderStatusChart() {
        var area = document.getElementById('statusChartArea');
        var data = dashData.statusDistribution || [];
        var max = Math.max.apply(null, data.map(function (d) { return d.count; }));
        if (max === 0) {
            area.innerHTML = '<span class="text-muted">Sem dados</span>';
            return;
        }

        var html = '';
        data.forEach(function (item) {
            var pct = Math.max(6, Math.round((item.count / max) * 100));
            html += '<div class="dash-bar-row">' +
                '<span class="dash-bar-label">' + esc(item.name) + '</span>' +
                '<div class="dash-bar-track"><div class="dash-bar-fill" style="width:' + pct + '%;background:' + statusColor(item.status) + ';">' + item.count + '</div></div>' +
                '</div>';
        });
        area.innerHTML = html;
    }

    // ---- Sector Ranking ----
    function renderSectorRanking() {
        var body = document.getElementById('sectorRankBody');
        var data = dashData.sectorRanking || [];
        if (data.length === 0) {
            body.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-3">Sem dados</td></tr>';
            return;
        }
        var html = '';
        data.forEach(function (s) {
            html += '<tr>' +
                '<td><span class="sector-dot" style="background:' + esc(s.color) + ';"></span>' + esc(s.name) + '</td>' +
                '<td class="text-center"><strong>' + s.totalCount + '</strong></td>' +
                '<td class="text-center"><span class="badge status-open">' + s.openCount + '</span></td>' +
                '<td class="text-center">' + fmtDuration(s.avgCloseTimeMinutes) + '</td>' +
                '</tr>';
        });
        body.innerHTML = html;
    }

    // ---- Type Ranking ----
    function renderTypeRanking() {
        var area = document.getElementById('typeRankArea');
        var data = dashData.typeRanking || [];
        if (data.length === 0) {
            area.innerHTML = '<span class="text-muted d-block text-center py-3">Sem dados</span>';
            return;
        }
        var html = '';
        data.forEach(function (t, i) {
            html += '<div class="rank-item">' +
                '<span class="rank-pos">' + (i + 1) + '.</span>' +
                '<span class="rank-name">' + esc(t.name) + '</span>' +
                '<span class="rank-count">' + t.count + '</span>' +
                '</div>';
        });
        area.innerHTML = html;
    }

    // ---- Requester Ranking ----
    function renderRequesterRanking() {
        var area = document.getElementById('requesterRankArea');
        var data = dashData.topRequesters || [];
        if (data.length === 0) {
            area.innerHTML = '<span class="text-muted d-block text-center py-3">Sem dados</span>';
            return;
        }
        var html = '';
        data.forEach(function (u, i) {
            html += '<div class="rank-item">' +
                '<span class="rank-pos">' + (i + 1) + '.</span>' +
                '<span class="rank-name">' + esc(u.name) + '</span>' +
                '<span class="rank-count">' + u.count + '</span>' +
                '</div>';
        });
        area.innerHTML = html;
    }

    // ---- Peak Hours ----
    function renderPeakHours() {
        var area = document.getElementById('peakHoursArea');
        var data = dashData.peakHours || [];
        if (data.length === 0) {
            area.innerHTML = '<span class="text-muted">Sem dados</span>';
            return;
        }
        var max = Math.max.apply(null, data.map(function (h) { return h.count; }));
        if (max === 0) {
            area.innerHTML = '<span class="text-muted">Sem dados suficientes</span>';
            return;
        }

        var html = '<div class="hour-bar-wrap">';
        data.forEach(function (h) {
            var pct = Math.max(2, Math.round((h.count / max) * 100));
            var intensity = h.count / max;
            var color = intensity > 0.7 ? '#f44336' : intensity > 0.4 ? '#ffc107' : '#1e9ff2';
            html += '<div class="hour-bar-col">' +
                '<div class="hour-bar" style="height:' + pct + '%;background:' + color + ';" title="' + h.count + ' chamados"></div>' +
                '<span class="hour-bar-label">' + (h.hour < 10 ? '0' : '') + h.hour + '</span>' +
                '</div>';
        });
        html += '</div>';
        area.innerHTML = html;
    }

    // ---- Month Compare ----
    function renderMonthCompare() {
        var area = document.getElementById('monthCompareArea');
        var d = dashData;

        var rows = [
            { label: 'Chamados Abertos', current: d.currentMonthCount, prev: d.previousMonthCount },
            { label: 'Chamados Encerrados', current: d.currentMonthClosedCount, prev: d.previousMonthClosedCount },
            { label: 'Tempo Médio Resolução', current: fmtDuration(d.avgCloseTimeCurrentMonth), prev: fmtDuration(d.avgCloseTimePreviousMonth), raw: true }
        ];

        var html = '';
        rows.forEach(function (r) {
            var trendHtml = '';
            if (!r.raw) {
                var diff = r.current - r.prev;
                if (diff > 0) trendHtml = '<span class="trend-up"><i class="la la-arrow-up"></i></span>';
                else if (diff < 0) trendHtml = '<span class="trend-down"><i class="la la-arrow-down"></i></span>';
            }

            html += '<div class="compare-row">' +
                '<span class="compare-label">' + esc(r.label) + '</span>' +
                '<div class="compare-values">' +
                '<span class="compare-current">' + (r.raw ? r.current : r.current) + ' ' + trendHtml + '</span><br/>' +
                '<span class="compare-prev">Mês anterior: ' + (r.raw ? r.prev : r.prev) + '</span>' +
                '</div>' +
                '</div>';
        });
        area.innerHTML = html;
    }

    // ---- Recent Table ----
    function renderRecentTable() {
        var body = document.getElementById('recentBody');
        var data = dashData.recentRequests || [];
        if (data.length === 0) {
            body.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-3">Nenhum chamado encontrado</td></tr>';
            return;
        }
        var html = '';
        data.forEach(function (r) {
            var minutes;
            if (r.closedAt) {
                minutes = Math.floor((new Date(r.closedAt) - new Date(r.createdAt)) / 60000);
            } else {
                minutes = Math.floor((new Date() - new Date(r.createdAt)) / 60000);
            }
            html += '<tr>' +
                '<td><span style="font-family:\'Courier New\',monospace;font-weight:700;color:#1e9ff2;">' + esc(r.code) + '</span></td>' +
                '<td><span class="sector-dot" style="background:' + esc(r.sectorColor) + ';"></span>' + esc(r.sectorName) + '</td>' +
                '<td>' + esc(r.typeName) + '</td>' +
                '<td>' + esc(r.areaName) + '</td>' +
                '<td>' + esc(r.requestedBy) + '</td>' +
                '<td><span class="status-badge ' + statusClass(r.status) + '">' + esc(r.statusName) + '</span></td>' +
                '<td>' + fmtDate(r.createdAt) + '</td>' +
                '<td>' + fmtDuration(minutes) + '</td>' +
                '</tr>';
        });
        body.innerHTML = html;
    }

    // ---- Init ----
    loadDashboard();
})();
