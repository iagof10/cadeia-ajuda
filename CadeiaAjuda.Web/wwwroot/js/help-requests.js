(function () {
    'use strict';

    // ---- State ----
    var currentUser = null;
    var requests = [];
    var sectors = [];
    var helpRequestTypes = [];
    var areas = [];

    // Wizard state
    var currentStep = 0;
    var selectedSectorId = null;
    var selectedTypeId = null;
    var selectedAreaId = null;

    // ---- DOM: Views ----
    var viewList = document.getElementById('viewList');
    var viewCreate = document.getElementById('viewCreate');
    var btnNewRequest = document.getElementById('btnNewRequest');
    var btnBackToList = document.getElementById('btnBackToList');

    // List
    var loadingList = document.getElementById('loadingList');
    var emptyList = document.getElementById('emptyList');
    var listCard = document.getElementById('listCard');
    var listBody = document.getElementById('listBody');
    var alertList = document.getElementById('alertList');

    // Wizard
    var wizardSteps = document.getElementById('wizardSteps');
    var panels = [
        document.getElementById('panel0'),
        document.getElementById('panel1'),
        document.getElementById('panel2'),
        document.getElementById('panel3')
    ];
    var btnSubmit = document.getElementById('btnSubmit');
    var alertCreate = document.getElementById('alertCreate');

    // Wizard panels
    var sectorGrid = document.getElementById('sectorGrid');
    var sectorLoading = document.getElementById('sectorLoading');
    var typeList = document.getElementById('typeList');
    var typeEmpty = document.getElementById('typeEmpty');
    var areaList = document.getElementById('areaList');
    var areaEmpty = document.getElementById('areaEmpty');

    // Summary
    var summSector = document.getElementById('summSector');
    var summType = document.getElementById('summType');
    var summArea = document.getElementById('summArea');

    // ---- Helpers ----
    function escapeHtml(str) {
        var div = document.createElement('div');
        div.textContent = str || '';
        return div.innerHTML;
    }

    function showListAlert(msg, type) {
        alertList.innerHTML =
            '<div class="alert alert-' + type + ' alert-dismissible mb-2" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            '<i class="la la-' + (type === 'success' ? 'check-circle' : 'warning') + '"></i> ' + escapeHtml(msg) +
            '</div>';
    }

    function showCreateAlert(msg, type) {
        alertCreate.innerHTML =
            '<div class="alert alert-' + type + ' alert-dismissible mb-2" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            '<i class="la la-' + (type === 'success' ? 'check-circle' : 'warning') + '"></i> ' + escapeHtml(msg) +
            '</div>';
    }

    function statusClass(status) {
        switch (status) {
            case 0: return 'status-open';
            case 1: return 'status-inprogress';
            case 2: return 'status-escalated';
            case 3: return 'status-resolved';
            case 4: return 'status-closed';
            default: return '';
        }
    }

    function formatDate(isoStr) {
        if (!isoStr) return '—';
        var d = new Date(isoStr);
        return d.toLocaleDateString('pt-BR') + ' ' + d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    }

    // ---- API ----
    async function fetchJson(url) {
        var resp = await fetch(url);
        if (!resp.ok) throw new Error('Erro ao carregar dados');
        return await resp.json();
    }

    async function loadCurrentUser() {
        try {
            currentUser = await fetchJson('/bff/me');
        } catch (e) {
            currentUser = null;
        }
    }

    async function loadRequests() {
        loadingList.style.display = '';
        emptyList.style.display = 'none';
        listCard.style.display = 'none';

        try {
            requests = await fetchJson('/bff/help-requests');
        } catch (e) {
            requests = [];
        }

        loadingList.style.display = 'none';
        renderList();
    }

    async function loadSectors() {
        try {
            sectors = await fetchJson('/bff/sectors');
        } catch (e) {
            sectors = [];
        }
    }

    async function loadHelpRequestTypes() {
        try {
            helpRequestTypes = await fetchJson('/bff/help-request-types');
        } catch (e) {
            helpRequestTypes = [];
        }
    }

    async function loadAreas() {
        try {
            areas = await fetchJson('/bff/areas');
        } catch (e) {
            areas = [];
        }
    }

    // ---- List ----
    var closingRequestId = null;

    function renderList() {
        listBody.innerHTML = '';

        if (requests.length === 0) {
            emptyList.style.display = '';
            listCard.style.display = 'none';
            return;
        }

        emptyList.style.display = 'none';
        listCard.style.display = '';

        requests.forEach(function (r) {
            var tr = document.createElement('tr');
            var isClosed = r.status === 4 || r.status === 3;
            var actionHtml = isClosed
                ? '<span class="text-muted"><i class="la la-check-circle"></i> Encerrado</span>'
                : '<button class="btn btn-sm btn-outline-danger btn-close-auth" data-id="' + r.id + '" data-code="' + escapeHtml(r.code) + '"><i class="la la-times-circle"></i> Encerrar</button>';

            tr.innerHTML =
                '<td><span class="help-request-code">' + escapeHtml(r.code) + '</span></td>' +
                '<td><span class="sector-dot" style="background-color:' + escapeHtml(r.sectorColor || '#999') + ';"></span>' + escapeHtml(r.sectorName) + '</td>' +
                '<td>' + escapeHtml(r.helpRequestTypeName) + '</td>' +
                '<td>' + escapeHtml(r.areaName) + '</td>' +
                '<td>' + escapeHtml(r.requestedByUserName) + '</td>' +
                '<td><span class="badge ' + statusClass(r.status) + '" style="font-size:.8rem;padding:4px 10px;border-radius:4px;">' + escapeHtml(r.statusName) + '</span></td>' +
                '<td>' + formatDate(r.createdAt) + '</td>' +
                '<td>' + actionHtml + '</td>';
            listBody.appendChild(tr);
        });

        // Bind close buttons to open the auth modal
        listBody.querySelectorAll('.btn-close-auth').forEach(function (btn) {
            btn.addEventListener('click', function () {
                closingRequestId = btn.getAttribute('data-id');
                document.getElementById('closeAuthCode').textContent = btn.getAttribute('data-code');
                document.getElementById('closeAuthLogin').value = '';
                document.getElementById('closeAuthPassword').value = '';
                document.getElementById('closeAuthAlert').innerHTML = '';
                $('#closeAuthModal').modal('show');
            });
        });
    }

    // ---- View switching ----
    function showListView() {
        viewList.style.display = '';
        viewCreate.style.display = 'none';
    }

    function showCreateView() {
        viewList.style.display = 'none';
        viewCreate.style.display = '';
        resetWizard();
    }

    // ---- Wizard ----
    function resetWizard() {
        currentStep = 0;
        selectedSectorId = null;
        selectedTypeId = null;
        selectedAreaId = null;
        alertCreate.innerHTML = '';
        updateWizardUI();
        renderSectors();
    }

    function updateWizardUI() {
        var tabs = wizardSteps.querySelectorAll('.nav-link');
        tabs.forEach(function (el, i) {
            el.classList.remove('active', 'done');
            if (i < currentStep) el.classList.add('done');
            if (i === currentStep) el.classList.add('active');
        });

        panels.forEach(function (p, i) {
            p.classList.toggle('active', i === currentStep);
        });

        btnSubmit.style.display = currentStep === 3 ? '' : 'none';
    }

    function goToStep(step) {
        alertCreate.innerHTML = '';
        currentStep = step;
        updateWizardUI();
    }

    // ---- Step 0: Sectors ----
    function renderSectors() {
        sectorLoading.style.display = 'none';
        sectorGrid.innerHTML = '';

        var activeSectors = sectors.filter(function (s) { return s.active; });

        if (activeSectors.length === 0) {
            sectorGrid.innerHTML = '<div class="text-center text-muted py-3">Nenhum setor cadastrado.</div>';
            return;
        }

        activeSectors.forEach(function (s) {
            var card = document.createElement('div');
            card.className = 'selection-card' + (selectedSectorId === s.id ? ' selected' : '');
            card.innerHTML =
                '<i class="la la-check check-mark"></i>' +
                '<div class="card-icon"><span class="sector-dot" style="background-color:' + escapeHtml(s.color || '#999') + ';width:32px;height:32px;display:inline-block;border-radius:50%;border:3px solid rgba(0,0,0,.1);"></span></div>' +
                '<div class="card-name">' + escapeHtml(s.name) + '</div>';
            card.addEventListener('click', function () {
                selectedSectorId = s.id;
                selectedTypeId = null;
                selectedAreaId = null;
                renderSectors();
                renderTypes();
                goToStep(1);
            });
            sectorGrid.appendChild(card);
        });
    }

    // ---- Step 1: Types ----
    function renderTypes() {
        typeList.innerHTML = '';
        typeEmpty.style.display = 'none';

        var filtered = helpRequestTypes.filter(function (t) {
            return t.sectorId === selectedSectorId && t.active;
        });

        if (filtered.length === 0) {
            typeEmpty.style.display = '';
            return;
        }

        filtered.forEach(function (t) {
            var item = document.createElement('div');
            item.className = 'type-list-item' + (selectedTypeId === t.id ? ' selected' : '');
            item.innerHTML =
                '<div class="type-radio"></div>' +
                '<div class="type-info">' +
                    '<div class="type-name">' + escapeHtml(t.name) + '</div>' +
                    (t.description ? '<div class="type-desc">' + escapeHtml(t.description) + '</div>' : '') +
                '</div>';
            item.addEventListener('click', function () {
                selectedTypeId = t.id;
                renderTypes();
                renderAreas();
                goToStep(2);
            });
            typeList.appendChild(item);
        });
    }

    // ---- Step 2: Areas (leaf nodes only) ----
    function getLeafAreas() {
        var parentIds = {};
        areas.forEach(function (a) {
            if (a.parentId) parentIds[a.parentId] = true;
        });

        return areas.filter(function (a) {
            return a.active && !parentIds[a.id];
        });
    }

    function getAreaPath(areaId) {
        var parts = [];
        var current = areas.find(function (a) { return a.id === areaId; });
        while (current) {
            parts.unshift(current.name);
            current = current.parentId ? areas.find(function (a) { return a.id === current.parentId; }) : null;
        }
        return parts.join(' > ');
    }

    function renderAreas() {
        areaList.innerHTML = '';
        areaEmpty.style.display = 'none';

        var leaves = getLeafAreas();

        if (leaves.length === 0) {
            areaEmpty.style.display = '';
            return;
        }

        leaves.forEach(function (a) {
            var item = document.createElement('div');
            item.className = 'area-leaf-card' + (selectedAreaId === a.id ? ' selected' : '');
            var path = getAreaPath(a.id);
            var pathParts = path.split(' > ');
            var parentPath = pathParts.length > 1 ? pathParts.slice(0, -1).join(' > ') : '';
            item.innerHTML =
                '<div class="area-radio"></div>' +
                '<div>' +
                    (parentPath ? '<div class="area-path">' + escapeHtml(parentPath) + '</div>' : '') +
                    '<div class="area-name">' + escapeHtml(a.name) + '</div>' +
                '</div>';
            item.addEventListener('click', function () {
                selectedAreaId = a.id;
                renderAreas();
                renderSummary();
                goToStep(3);
            });
            areaList.appendChild(item);
        });
    }

    // ---- Step 3: Summary ----
    function renderSummary() {
        var sector = sectors.find(function (s) { return s.id === selectedSectorId; });
        var type = helpRequestTypes.find(function (t) { return t.id === selectedTypeId; });
        var area = areas.find(function (a) { return a.id === selectedAreaId; });

        summSector.innerHTML = sector
            ? '<span class="sector-dot" style="background-color:' + escapeHtml(sector.color || '#999') + ';"></span>' + escapeHtml(sector.name)
            : '—';
        summType.textContent = type ? type.name : '—';
        summArea.textContent = area ? getAreaPath(area.id) : '—';
    }

    // ---- Submit ----
    async function handleSubmit() {
        alertCreate.innerHTML = '';

        if (!currentUser) {
            showCreateAlert('Usuário não autenticado.', 'danger');
            return;
        }

        var payload = {
            description: '',
            sectorId: selectedSectorId,
            helpRequestTypeId: selectedTypeId,
            areaId: selectedAreaId,
            requestedByUserId: currentUser.id,
            tenantId: currentUser.tenantId
        };

        btnSubmit.disabled = true;
        btnSubmit.innerHTML = '<i class="la la-spinner la-spin"></i> Abrindo...';

        try {
            var resp = await fetch('/bff/help-requests', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (resp.ok) {
                showListView();
                await loadRequests();
                showListAlert('Chamado aberto com sucesso!', 'success');
            } else {
                var body = {};
                try { body = await resp.json(); } catch (e) { }
                showCreateAlert(body.error || 'Ocorreu um erro ao abrir o chamado.', 'danger');
            }
        } catch (e) {
            showCreateAlert('Erro de conexão.', 'danger');
        }

        btnSubmit.disabled = false;
        btnSubmit.innerHTML = '<i class="la la-check"></i> Abrir Chamado';
    }

    // ---- Events ----
    btnNewRequest.addEventListener('click', showCreateView);
    btnBackToList.addEventListener('click', showListView);
    btnSubmit.addEventListener('click', handleSubmit);

    // ---- Close with auth modal ----
    var btnConfirmCloseAuth = document.getElementById('btnConfirmCloseAuth');

    btnConfirmCloseAuth.addEventListener('click', async function () {
        var login = document.getElementById('closeAuthLogin').value.trim();
        var password = document.getElementById('closeAuthPassword').value;
        var alertEl = document.getElementById('closeAuthAlert');
        alertEl.innerHTML = '';

        if (!login || !password) {
            alertEl.innerHTML = '<div class="alert alert-warning mb-2"><i class="la la-warning"></i> Informe usuário e senha.</div>';
            return;
        }

        if (!closingRequestId) return;

        btnConfirmCloseAuth.disabled = true;
        btnConfirmCloseAuth.innerHTML = '<i class="la la-spinner la-spin"></i> Encerrando...';

        try {
            var resp = await fetch('/bff/help-requests/' + closingRequestId + '/close-with-auth', {
                method: 'PATCH',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ login: login, password: password })
            });

            if (resp.ok) {
                $('#closeAuthModal').modal('hide');
                await loadRequests();
                showListAlert('Chamado encerrado com sucesso!', 'success');
            } else {
                var body = {};
                try { body = await resp.json(); } catch (e) { }
                alertEl.innerHTML = '<div class="alert alert-danger mb-2"><i class="la la-warning"></i> ' + escapeHtml(body.error || 'Usuário ou senha inválidos.') + '</div>';
            }
        } catch (e) {
            alertEl.innerHTML = '<div class="alert alert-danger mb-2"><i class="la la-warning"></i> Erro de conexão.</div>';
        }

        btnConfirmCloseAuth.disabled = false;
        btnConfirmCloseAuth.innerHTML = '<i class="la la-check"></i> Encerrar';
    });

    // Allow clicking wizard tabs to go back
    wizardSteps.querySelectorAll('.nav-link').forEach(function (el) {
        el.addEventListener('click', function () {
            var step = parseInt(el.getAttribute('data-step'));
            if (step < currentStep) {
                goToStep(step);
            }
        });
    });

    // ---- Init ----
    (async function init() {
        try {
            await loadCurrentUser();
            await Promise.all([loadRequests(), loadSectors(), loadHelpRequestTypes(), loadAreas()]);
        } catch (e) {
            showListAlert('Erro ao carregar dados iniciais. Recarregue a página.', 'danger');
        }
    })();
})();
