(function () {
    'use strict';

    var dashData = null;
    var chartInstances = {};

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

    var palette = ['#f57c00','#1e9ff2','#4caf50','#f44336','#9c27b0','#00bcd4','#ff5722','#607d8b','#e91e63','#8bc34a','#3f51b5','#ffc107'];

    function destroyChart(id) {
        if (chartInstances[id]) { chartInstances[id].destroy(); delete chartInstances[id]; }
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
            renderSectorPieChart();
            renderTypePieChart();
            renderPeakHoursChart();
            renderSectorRanking();
            renderTypeRanking();
            renderRequesterRanking();
            renderMonthCompare();
            renderMonthCompareChart();
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
        document.getElementById('kpiAvgTime').textContent = fmtDuration(d.avgCloseTimeMinutes);
        document.getElementById('kpiTotal').textContent = d.totalCount;
        document.getElementById('kpiUsers').textContent = d.activeUsersCount;
        document.getElementById('kpiOldest').textContent = d.openCount > 0 ? fmtDuration(d.oldestOpenMinutes) : '\u2014';

        document.getElementById('kpiMonthCompare').textContent = d.currentMonthCount;
        document.getElementById('kpiPrevMonth').textContent = d.previousMonthCount;
    }

    // ---- Period Chart (Bar) ----
    function renderPeriodChart(period) {
        var data;
        if (period === 'week') data = dashData.last7Days;
        else if (period === 'month') data = dashData.last4Weeks;
        else data = dashData.last6Months;

        destroyChart('periodChart');
        var ctx = document.getElementById('periodChart');
        if (!data || data.length === 0) return;

        chartInstances['periodChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(function (d) { return d.label; }),
                datasets: [{
                    label: 'Chamados',
                    data: data.map(function (d) { return d.count; }),
                    backgroundColor: 'rgba(30,159,242,0.7)',
                    borderColor: '#1e9ff2',
                    borderWidth: 2,
                    borderRadius: 6,
                    barPercentage: 0.6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { stepSize: 1, precision: 0 } }
                }
            }
        });
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

    // ---- Status Chart (Pie) ----
    function renderStatusChart() {
        destroyChart('statusChart');
        var ctx = document.getElementById('statusChart');
        var data = dashData.statusDistribution || [];
        if (data.length === 0) return;

        chartInstances['statusChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: data.map(function (d) { return d.name; }),
                datasets: [{
                    data: data.map(function (d) { return d.count; }),
                    backgroundColor: data.map(function (d) { return statusColor(d.status); }),
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 12, usePointStyle: true, pointStyle: 'circle' } }
                }
            }
        });
    }

    // ---- Sector Pie Chart ----
    function renderSectorPieChart() {
        destroyChart('sectorPieChart');
        var ctx = document.getElementById('sectorPieChart');
        var data = dashData.sectorRanking || [];
        if (data.length === 0) return;

        chartInstances['sectorPieChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: data.map(function (d) { return d.name; }),
                datasets: [{
                    data: data.map(function (d) { return d.totalCount; }),
                    backgroundColor: data.map(function (d, i) { return d.color || palette[i % palette.length]; }),
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 12, usePointStyle: true, pointStyle: 'circle' } }
                }
            }
        });
    }

    // ---- Type Doughnut Chart ----
    function renderTypePieChart() {
        destroyChart('typePieChart');
        var ctx = document.getElementById('typePieChart');
        var data = dashData.typeRanking || [];
        if (data.length === 0) return;

        chartInstances['typePieChart'] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: data.map(function (d) { return d.name; }),
                datasets: [{
                    data: data.map(function (d) { return d.count; }),
                    backgroundColor: data.map(function (d, i) { return palette[i % palette.length]; }),
                    borderWidth: 2,
                    borderColor: '#fff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 12, usePointStyle: true, pointStyle: 'circle' } }
                }
            }
        });
    }

    // ---- Peak Hours Chart (Bar) ----
    function renderPeakHoursChart() {
        destroyChart('peakHoursChart');
        var ctx = document.getElementById('peakHoursChart');
        var data = dashData.peakHours || [];
        if (data.length === 0) return;

        var max = Math.max.apply(null, data.map(function (h) { return h.count; }));

        chartInstances['peakHoursChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(function (h) { return (h.hour < 10 ? '0' : '') + h.hour + 'h'; }),
                datasets: [{
                    label: 'Chamados',
                    data: data.map(function (h) { return h.count; }),
                    backgroundColor: data.map(function (h) {
                        var intensity = max > 0 ? h.count / max : 0;
                        return intensity > 0.7 ? '#f44336' : intensity > 0.4 ? '#ffc107' : '#1e9ff2';
                    }),
                    borderRadius: 4,
                    barPercentage: 0.7
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { stepSize: 1, precision: 0 } },
                    x: { ticks: { font: { size: 10 } } }
                }
            }
        });
    }

    // ---- Month Compare Chart (Grouped Bar) ----
    function renderMonthCompareChart() {
        destroyChart('monthCompareChart');
        var ctx = document.getElementById('monthCompareChart');
        var d = dashData;

        chartInstances['monthCompareChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Abertos', 'Encerrados'],
                datasets: [
                    {
                        label: 'Mês Atual',
                        data: [d.currentMonthCount, d.currentMonthClosedCount],
                        backgroundColor: 'rgba(245,124,0,0.8)',
                        borderRadius: 6,
                        barPercentage: 0.5
                    },
                    {
                        label: 'Mês Anterior',
                        data: [d.previousMonthCount, d.previousMonthClosedCount],
                        backgroundColor: 'rgba(158,158,158,0.5)',
                        borderRadius: 6,
                        barPercentage: 0.5
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom', labels: { usePointStyle: true, pointStyle: 'circle' } }
                },
                scales: {
                    y: { beginAtZero: true, ticks: { stepSize: 1, precision: 0 } }
                }
            }
        });
    }

    // ---- Sector Ranking (Table) ----
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

    // ---- Type Ranking (List) ----
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

    // ---- Month Compare (Text) ----
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
                '<span class="compare-current">' + r.current + ' ' + trendHtml + '</span><br/>' +
                '<span class="compare-prev">Mês anterior: ' + r.prev + '</span>' +
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
