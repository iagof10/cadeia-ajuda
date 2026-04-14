(function () {
    'use strict';

    // ---- State ----
    var sectors = [];
    var users = [];
    var levels = [];
    var selectedSectorId = null;
    var editingId = null;
    var deletingId = null;
    var responsibles = [];

    // ---- DOM refs ----
    var sectorList = document.getElementById('sectorList');
    var sectorListLoading = document.getElementById('sectorListLoading');
    var noSectorSelected = document.getElementById('noSectorSelected');
    var configArea = document.getElementById('configArea');
    var selectedSectorName = document.getElementById('selectedSectorName');
    var selectedSectorDot = document.getElementById('selectedSectorDot');
    var btnAddLevel = document.getElementById('btnAddLevel');
    var loadingArea = document.getElementById('loadingArea');
    var emptyState = document.getElementById('emptyState');
    var levelsContainer = document.getElementById('levelsContainer');
    var flowCard = document.getElementById('flowCard');
    var flowContainer = document.getElementById('flowContainer');
    var alertArea = document.getElementById('alertArea');

    // Modal refs
    var modalTitle = document.getElementById('modalTitle');
    var modalAlert = document.getElementById('modalAlert');
    var fName = document.getElementById('fName');
    var fOrder = document.getElementById('fOrder');
    var fTime = document.getElementById('fTime');
    var fDesc = document.getElementById('fDesc');
    var fUserSelect = document.getElementById('fUserSelect');
    var fIsPrimary = document.getElementById('fIsPrimary');
    var btnAddResp = document.getElementById('btnAddResp');
    var btnSave = document.getElementById('btnSave');
    var btnSaveText = document.getElementById('btnSaveText');
    var respListContainer = document.getElementById('respListContainer');
    var noRespMsg = document.getElementById('noRespMsg');

    // Delete modal
    var deleteNameLabel = document.getElementById('deleteNameLabel');
    var btnConfirmDelete = document.getElementById('btnConfirmDelete');

    // ---- Helpers ----
    function formatTime(minutes) {
        if (minutes < 60) return minutes + ' min';
        var h = Math.floor(minutes / 60);
        var m = minutes % 60;
        return m > 0 ? h + 'h ' + m + 'min' : h + 'h';
    }

    function showAlert(msg, type) {
        alertArea.innerHTML =
            '<div class="alert alert-' + type + ' alert-dismissible mb-2" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            '<i class="la la-' + (type === 'success' ? 'check-circle' : 'warning') + '"></i> ' + escapeHtml(msg) +
            '</div>';
    }

    function escapeHtml(str) {
        var div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }

    function getUserName(userId) {
        var u = users.find(function (x) { return x.id === userId; });
        return u ? u.name : '—';
    }

    // ---- API calls ----
    async function fetchJson(url) {
        var resp = await fetch(url);
        if (!resp.ok) throw new Error('Erro ao carregar dados');
        return await resp.json();
    }

    // ---- Sectors ----
    async function loadSectors() {
        sectors = await fetchJson('/bff/sectors');
        renderSectorList();
    }

    function renderSectorList() {
        sectorListLoading.style.display = 'none';
        sectorList.innerHTML = '';

        var activeSectors = sectors.filter(function (s) { return s.active; });

        if (activeSectors.length === 0) {
            sectorList.innerHTML = '<div class="text-muted small text-center py-3">Nenhum setor cadastrado.</div>';
            return;
        }

        activeSectors.forEach(function (s) {
            var item = document.createElement('div');
            item.className = 'sector-list-item' + (selectedSectorId === s.id ? ' active' : '');
            item.setAttribute('data-id', s.id);
            item.innerHTML =
                '<span class="sector-color-dot" style="background-color:' + escapeHtml(s.color || '#999') + ';"></span>' +
                '<span class="sector-name">' + escapeHtml(s.name) + '</span>';
            item.addEventListener('click', function () {
                selectSector(s.id);
            });
            sectorList.appendChild(item);
        });
    }

    function selectSector(sectorId) {
        selectedSectorId = sectorId;
        var sector = sectors.find(function (s) { return s.id === sectorId; });
        if (!sector) return;

        // Update left panel active state
        document.querySelectorAll('.sector-list-item').forEach(function (el) {
            el.classList.toggle('active', el.getAttribute('data-id') === sectorId);
        });

        // Show right panel
        noSectorSelected.style.display = 'none';
        configArea.style.display = '';
        selectedSectorName.textContent = sector.name;
        selectedSectorDot.style.backgroundColor = sector.color || '#999';

        loadLevels();
    }

    async function loadUsers() {
        users = await fetchJson('/bff/users');
    }

    async function loadLevels() {
        if (!selectedSectorId) return;
        loadingArea.style.display = '';
        levelsContainer.innerHTML = '';
        emptyState.style.display = 'none';
        flowCard.style.display = 'none';

        try {
            levels = await fetchJson('/bff/escalation-levels/by-sector/' + selectedSectorId);
        } catch (e) {
            levels = [];
        }

        loadingArea.style.display = 'none';
        renderLevels();
    }

    // ---- Render Levels ----
    function renderLevels() {
        levelsContainer.innerHTML = '';

        if (levels.length === 0) {
            emptyState.style.display = '';
            flowCard.style.display = 'none';
            return;
        }

        emptyState.style.display = 'none';

        var sorted = levels.slice().sort(function (a, b) { return a.order - b.order; });

        sorted.forEach(function (level) {
            var respBadges = '';
            if (level.responsibles && level.responsibles.length > 0) {
                var sortedResp = level.responsibles.slice().sort(function (a, b) { return (b.isPrimary ? 1 : 0) - (a.isPrimary ? 1 : 0); });
                sortedResp.forEach(function (r) {
                    respBadges += '<span class="badge ' + (r.isPrimary ? 'badge-success' : 'badge-light') + ' mr-1 mb-1">' +
                        (r.isPrimary ? '? ' : '') + escapeHtml(r.userName) + '</span>';
                });
            } else {
                respBadges = '<span class="text-muted small"><em>Nenhum responsável definido</em></span>';
            }

            var inactiveFooter = '';
            if (!level.active) {
                inactiveFooter = '<div class="card-footer py-1 bg-light"><span class="badge badge-danger">Inativo</span></div>';
            }

            var html =
                '<div class="col-md-4 mb-2">' +
                '<div class="card ' + (!level.active ? 'border-danger' : '') + '">' +
                '<div class="card-header d-flex justify-content-between align-items-center py-1">' +
                '<div>' +
                '<span class="badge badge-pill ' + (level.active ? 'badge-primary' : 'badge-danger') + ' mr-1">Nível ' + level.order + '</span>' +
                '<strong>' + escapeHtml(level.name) + '</strong>' +
                '</div>' +
                '<div class="dropdown">' +
                '<button type="button" class="btn btn-sm btn-link p-0 text-secondary esc-actions-toggle" data-toggle="dropdown"><i class="ft-more-vertical"></i></button>' +
                '<div class="dropdown-menu dropdown-menu-right">' +
                '<button class="dropdown-item btn-edit-level" data-id="' + level.id + '"><i class="ft-edit-2 mr-1"></i> Editar</button>' +
                '<button class="dropdown-item text-danger btn-delete-level" data-id="' + level.id + '"><i class="ft-trash-2 mr-1"></i> Excluir</button>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div class="card-body pt-1 pb-2">' +
                (level.description ? '<p class="text-muted small mb-1">' + escapeHtml(level.description) + '</p>' : '') +
                '<div class="mb-1"><i class="la la-clock-o text-primary"></i> <span class="small">Tempo para escalonar: <strong>' + formatTime(level.escalationTimeMinutes) + '</strong></span></div>' +
                '<div class="mt-1"><span class="small font-weight-bold d-block mb-1"><i class="la la-users text-primary"></i> Responsáveis:</span>' +
                respBadges +
                '</div>' +
                '</div>' +
                inactiveFooter +
                '</div>' +
                '</div>';

            levelsContainer.insertAdjacentHTML('beforeend', html);
        });

        // Flow
        var activeLevels = sorted.filter(function (l) { return l.active; });
        if (activeLevels.length > 0) {
            flowCard.style.display = '';
            flowContainer.innerHTML = '';
            activeLevels.forEach(function (level, idx) {
                var primary = (level.responsibles || []).find(function (r) { return r.isPrimary; });
                var primaryHtml = primary ? '<small class="d-block text-success">? ' + escapeHtml(primary.userName) + '</small>' : '';

                flowContainer.insertAdjacentHTML('beforeend',
                    '<div class="escalation-flow-step">' +
                    '<div class="escalation-flow-badge">' + level.order + '</div>' +
                    '<div class="escalation-flow-info">' +
                    '<strong>' + escapeHtml(level.name) + '</strong>' +
                    '<small class="d-block text-muted">' + formatTime(level.escalationTimeMinutes) + '</small>' +
                    primaryHtml +
                    '</div>' +
                    '</div>'
                );

                if (idx < activeLevels.length - 1) {
                    flowContainer.insertAdjacentHTML('beforeend',
                        '<div class="escalation-flow-arrow"><i class="la la-arrow-right"></i></div>'
                    );
                }
            });
        } else {
            flowCard.style.display = 'none';
        }

        // Bind edit/delete buttons
        document.querySelectorAll('.btn-edit-level').forEach(function (btn) {
            btn.addEventListener('click', function () {
                openEditModal(this.getAttribute('data-id'));
            });
        });
        document.querySelectorAll('.btn-delete-level').forEach(function (btn) {
            btn.addEventListener('click', function () {
                openDeleteModal(this.getAttribute('data-id'));
            });
        });
    }

    // ---- Modal logic ----
    function resetModal() {
        fName.value = '';
        fOrder.value = '1';
        fTime.value = '30';
        fDesc.value = '';
        fIsPrimary.checked = false;
        responsibles = [];
        editingId = null;
        modalAlert.innerHTML = '';
        renderResponsiblesTable();
        refreshUserDropdown();
    }

    function refreshUserDropdown() {
        fUserSelect.innerHTML = '<option value="">— Selecione um usuário —</option>';
        users.filter(function (u) {
            return u.active && !responsibles.some(function (r) { return r.userId === u.id; });
        }).forEach(function (u) {
            var opt = document.createElement('option');
            opt.value = u.id;
            opt.textContent = u.name;
            fUserSelect.appendChild(opt);
        });
    }

    function getInitials(name) {
        if (!name) return '?';
        var parts = name.trim().split(/\s+/);
        if (parts.length >= 2) return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
        return parts[0].substring(0, 2).toUpperCase();
    }

    function renderResponsiblesTable() {
        respListContainer.innerHTML = '';
        if (responsibles.length === 0) {
            noRespMsg.style.display = '';
            respListContainer.style.display = 'none';
            return;
        }
        noRespMsg.style.display = 'none';
        respListContainer.style.display = '';

        responsibles.forEach(function (resp, idx) {
            var name = getUserName(resp.userId);
            var initials = getInitials(name);
            var primaryTag = resp.isPrimary
                ? '<span class="resp-role-tag" style="background:#d4edda;color:#155724;">? Principal</span>'
                : '';
            var starClass = resp.isPrimary ? 'btn-star active' : 'btn-star';
            var starTitle = resp.isPrimary ? 'Responsável principal' : 'Definir como principal';

            respListContainer.insertAdjacentHTML('beforeend',
                '<div class="resp-card' + (resp.isPrimary ? ' is-primary' : '') + '" data-idx="' + idx + '">' +
                '<div class="resp-avatar">' + escapeHtml(initials) + '</div>' +
                '<div class="resp-info">' +
                '<span class="resp-name">' + escapeHtml(name) + '</span>' +
                primaryTag +
                '</div>' +
                '<div class="resp-actions">' +
                '<button type="button" class="' + starClass + ' btn-set-primary" data-idx="' + idx + '" title="' + starTitle + '"><i class="la la-star"></i></button>' +
                '<button type="button" class="btn-trash btn-remove-resp" data-idx="' + idx + '" title="Remover"><i class="la la-trash"></i></button>' +
                '</div>' +
                '</div>'
            );
        });

        document.querySelectorAll('.btn-set-primary').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var i = parseInt(this.getAttribute('data-idx'));
                responsibles.forEach(function (r) { r.isPrimary = false; });
                responsibles[i].isPrimary = true;
                renderResponsiblesTable();
            });
        });
        document.querySelectorAll('.btn-remove-resp').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var i = parseInt(this.getAttribute('data-idx'));
                responsibles.splice(i, 1);
                renderResponsiblesTable();
                refreshUserDropdown();
            });
        });
    }

    function openAddModal() {
        resetModal();
        var nextOrder = levels.length > 0 ? Math.max.apply(null, levels.map(function (l) { return l.order; })) + 1 : 1;
        fOrder.value = nextOrder;
        modalTitle.textContent = 'Novo Nível de Escalonamento';
        btnSaveText.textContent = 'Cadastrar';
        $('#levelModal').modal('show');
    }

    function openEditModal(id) {
        resetModal();
        var level = levels.find(function (l) { return l.id === id; });
        if (!level) return;

        editingId = id;
        fName.value = level.name;
        fOrder.value = level.order;
        fTime.value = level.escalationTimeMinutes;
        fDesc.value = level.description || '';
        responsibles = (level.responsibles || []).map(function (r) {
            return { userId: r.userId, isPrimary: r.isPrimary };
        });

        modalTitle.textContent = 'Editar Nível de Escalonamento';
        btnSaveText.textContent = 'Salvar Alterações';
        renderResponsiblesTable();
        refreshUserDropdown();
        $('#levelModal').modal('show');
    }

    function openDeleteModal(id) {
        var level = levels.find(function (l) { return l.id === id; });
        if (!level) return;
        deletingId = id;
        deleteNameLabel.textContent = '"' + level.name + '"';
        $('#deleteModal').modal('show');
    }

    // ---- Save ----
    async function handleSave() {
        modalAlert.innerHTML = '';

        var name = fName.value.trim();
        if (!name) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> Informe o nome do nível.</div>';
            return;
        }
        var order = parseInt(fOrder.value);
        var time = parseInt(fTime.value);
        if (!order || order < 1) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> A ordem deve ser maior que zero.</div>';
            return;
        }
        if (!time || time < 1) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> O tempo deve ser maior que zero.</div>';
            return;
        }

        var sector = sectors.find(function (s) { return s.id === selectedSectorId; });
        if (!sector) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> Setor inválido.</div>';
            return;
        }

        var payload = {
            name: name,
            description: fDesc.value.trim(),
            order: order,
            escalationTimeMinutes: time,
            sectorId: selectedSectorId,
            tenantId: sector.tenantId,
            active: true,
            responsibles: responsibles.map(function (r) {
                return { userId: r.userId, isPrimary: r.isPrimary };
            })
        };

        btnSave.disabled = true;
        btnSaveText.textContent = 'Salvando...';

        try {
            var url, method;
            if (editingId) {
                url = '/bff/escalation-levels/' + editingId;
                method = 'PUT';
            } else {
                url = '/bff/escalation-levels';
                method = 'POST';
            }

            var resp = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (resp.ok) {
                $('#levelModal').modal('hide');
                showAlert(editingId ? 'Nível atualizado com sucesso.' : 'Nível criado com sucesso.', 'success');
                await loadLevels();
            } else {
                var body = {};
                try { body = await resp.json(); } catch (e) { }
                modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> ' + escapeHtml(body.error || 'Ocorreu um erro ao salvar.') + '</div>';
            }
        } catch (e) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> Erro de conexão.</div>';
        }

        btnSave.disabled = false;
        btnSaveText.textContent = editingId ? 'Salvar Alterações' : 'Cadastrar';
    }

    // ---- Delete ----
    async function handleDelete() {
        if (!deletingId) return;
        btnConfirmDelete.disabled = true;

        try {
            var resp = await fetch('/bff/escalation-levels/' + deletingId, { method: 'DELETE' });
            if (resp.ok) {
                $('#deleteModal').modal('hide');
                showAlert('Nível excluído com sucesso.', 'success');
                await loadLevels();
            } else {
                showAlert('Ocorreu um erro ao excluir o nível.', 'danger');
                $('#deleteModal').modal('hide');
            }
        } catch (e) {
            showAlert('Erro de conexão.', 'danger');
            $('#deleteModal').modal('hide');
        }

        btnConfirmDelete.disabled = false;
        deletingId = null;
    }

    // ---- Events ----
    btnAddLevel.addEventListener('click', openAddModal);

    btnAddResp.addEventListener('click', function () {
        var userId = fUserSelect.value;
        if (!userId) return;
        if (responsibles.some(function (r) { return r.userId === userId; })) return;

        var isPrimary = fIsPrimary.checked;
        if (isPrimary) {
            responsibles.forEach(function (r) { r.isPrimary = false; });
        }

        responsibles.push({ userId: userId, isPrimary: isPrimary });
        fIsPrimary.checked = false;
        renderResponsiblesTable();
        refreshUserDropdown();
    });

    btnSave.addEventListener('click', handleSave);
    btnConfirmDelete.addEventListener('click', handleDelete);

    // ---- Init ----
    (async function init() {
        try {
            await Promise.all([loadSectors(), loadUsers()]);
        } catch (e) {
            showAlert('Erro ao carregar dados iniciais. Recarregue a página.', 'danger');
        }
    })();
})();
