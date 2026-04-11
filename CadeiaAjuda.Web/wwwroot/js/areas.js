(function () {
    'use strict';

    // ---- State ----
    var areas = [];
    var tenants = [];
    var editingId = null;
    var deletingId = null;
    var expandedNodes = {};
    var preselectedParentId = null;

    // ---- DOM refs ----
    var loadingArea = document.getElementById('loadingArea');
    var emptyState = document.getElementById('emptyState');
    var treeCard = document.getElementById('treeCard');
    var treeContainer = document.getElementById('treeContainer');
    var alertArea = document.getElementById('alertArea');
    var btnAddRoot = document.getElementById('btnAddRoot');

    // Modal refs
    var modalTitle = document.getElementById('modalTitle');
    var modalSubtitle = document.getElementById('modalSubtitle');
    var modalAlert = document.getElementById('modalAlert');
    var fName = document.getElementById('fName');
    var fDesc = document.getElementById('fDesc');
    var fTenant = document.getElementById('fTenant');
    var fParent = document.getElementById('fParent');
    var hierarchyPreview = document.getElementById('hierarchyPreview');
    var hierarchyPath = document.getElementById('hierarchyPath');
    var btnSave = document.getElementById('btnSave');
    var btnSaveText = document.getElementById('btnSaveText');

    // Delete modal
    var deleteNameLabel = document.getElementById('deleteNameLabel');
    var btnConfirmDelete = document.getElementById('btnConfirmDelete');

    // ---- Helpers ----
    function escapeHtml(str) {
        var div = document.createElement('div');
        div.textContent = str || '';
        return div.innerHTML;
    }

    function showAlert(msg, type) {
        alertArea.innerHTML =
            '<div class="alert alert-' + type + ' alert-dismissible mb-2" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            '<i class="la la-' + (type === 'success' ? 'check-circle' : 'warning') + '"></i> ' + escapeHtml(msg) +
            '</div>';
    }

    function getChildren(parentId) {
        return areas.filter(function (a) { return a.parentId === parentId; });
    }

    function getRoots() {
        return areas.filter(function (a) {
            return a.parentId === null || a.parentId === undefined || !areas.some(function (p) { return p.id === a.parentId; });
        });
    }

    function getAncestorPath(parentId) {
        var parts = [];
        var current = areas.find(function (a) { return a.id === parentId; });
        while (current) {
            parts.unshift(current.name);
            current = current.parentId ? areas.find(function (a) { return a.id === current.parentId; }) : null;
        }
        return parts;
    }

    // ---- API ----
    async function fetchJson(url) {
        var resp = await fetch(url);
        if (!resp.ok) throw new Error('Erro ao carregar dados');
        return await resp.json();
    }

    async function loadAreas() {
        loadingArea.style.display = '';
        emptyState.style.display = 'none';
        treeCard.style.display = 'none';

        try {
            areas = await fetchJson('/bff/areas');
        } catch (e) {
            areas = [];
        }

        loadingArea.style.display = 'none';
        renderTree();
    }

    async function loadTenants() {
        try {
            tenants = await fetchJson('/bff/tenants');
        } catch (e) {
            tenants = [];
        }
    }

    // ---- Render tree ----
    function renderTree() {
        treeContainer.innerHTML = '';
        var roots = getRoots();

        if (roots.length === 0) {
            emptyState.style.display = '';
            treeCard.style.display = 'none';
            return;
        }

        emptyState.style.display = 'none';
        treeCard.style.display = '';

        roots.forEach(function (area) {
            treeContainer.appendChild(buildTreeNode(area));
        });
    }

    function buildTreeNode(area) {
        var children = getChildren(area.id);
        var hasChildren = children.length > 0;
        var isExpanded = expandedNodes[area.id] !== false; // default expanded

        var node = document.createElement('div');
        node.className = 'area-tree-node';

        // Header
        var header = document.createElement('div');
        header.className = 'area-tree-header';

        if (hasChildren) {
            var toggle = document.createElement('button');
            toggle.type = 'button';
            toggle.className = 'area-tree-toggle' + (isExpanded ? ' expanded' : '');
            toggle.innerHTML = '<i class="ft-chevron-right"></i>';
            toggle.addEventListener('click', function () {
                expandedNodes[area.id] = !isExpanded;
                renderTree();
            });
            header.appendChild(toggle);
        } else {
            var leaf = document.createElement('span');
            leaf.className = 'area-leaf-icon';
            leaf.innerHTML = '<i class="ft-circle" style="font-size:.5rem;"></i>';
            header.appendChild(leaf);
        }

        var nameSpan = document.createElement('span');
        nameSpan.className = 'area-tree-name';
        nameSpan.textContent = area.name;
        header.appendChild(nameSpan);

        if (area.description) {
            var descSpan = document.createElement('span');
            descSpan.className = 'area-tree-desc';
            descSpan.textContent = '— ' + area.description;
            header.appendChild(descSpan);
        }

        if (hasChildren) {
            var countBadge = document.createElement('span');
            countBadge.className = 'area-node-badge badge badge-light';
            countBadge.textContent = children.length + ' sub-área(s)';
            header.appendChild(countBadge);
        } else {
            var leafBadge = document.createElement('span');
            leafBadge.className = 'area-node-badge badge badge-info';
            leafBadge.innerHTML = '<i class="ft-target" style="font-size:.65rem;"></i> nó final';
            header.appendChild(leafBadge);
        }

        var statusBadge = document.createElement('span');
        statusBadge.className = 'badge ' + (area.active ? 'badge-success' : 'badge-danger');
        statusBadge.style.fontSize = '.7rem';
        statusBadge.textContent = area.active ? 'Ativo' : 'Inativo';
        header.appendChild(statusBadge);

        // Actions
        var actions = document.createElement('div');
        actions.className = 'area-tree-actions';

        var btnAddChild = document.createElement('button');
        btnAddChild.type = 'button';
        btnAddChild.className = 'btn btn-sm btn-outline-primary';
        btnAddChild.title = 'Adicionar sub-área';
        btnAddChild.innerHTML = '<i class="ft-plus"></i>';
        btnAddChild.addEventListener('click', function () { openAddModal(area.id); });
        actions.appendChild(btnAddChild);

        var btnEdit = document.createElement('button');
        btnEdit.type = 'button';
        btnEdit.className = 'btn btn-sm btn-outline-info';
        btnEdit.title = 'Editar';
        btnEdit.innerHTML = '<i class="ft-edit-2"></i>';
        btnEdit.addEventListener('click', function () { openEditModal(area.id); });
        actions.appendChild(btnEdit);

        var btnToggle = document.createElement('button');
        btnToggle.type = 'button';
        btnToggle.className = 'btn btn-sm ' + (area.active ? 'btn-outline-warning' : 'btn-outline-success');
        btnToggle.title = area.active ? 'Inativar' : 'Ativar';
        btnToggle.innerHTML = '<i class="ft-power"></i>';
        btnToggle.addEventListener('click', function () { toggleActive(area); });
        actions.appendChild(btnToggle);

        var btnDelete = document.createElement('button');
        btnDelete.type = 'button';
        btnDelete.className = 'btn btn-sm btn-outline-danger';
        btnDelete.title = 'Excluir';
        btnDelete.innerHTML = '<i class="ft-trash-2"></i>';
        btnDelete.addEventListener('click', function () { openDeleteModal(area.id); });
        actions.appendChild(btnDelete);

        header.appendChild(actions);
        node.appendChild(header);

        // Children
        if (hasChildren && isExpanded) {
            var childContainer = document.createElement('div');
            childContainer.className = 'area-tree-children';
            children.forEach(function (child) {
                childContainer.appendChild(buildTreeNode(child));
            });
            node.appendChild(childContainer);
        }

        return node;
    }

    // ---- Modal ----
    function resetModal() {
        fName.value = '';
        fDesc.value = '';
        fTenant.value = '';
        fParent.value = '';
        editingId = null;
        preselectedParentId = null;
        modalAlert.innerHTML = '';
        hierarchyPreview.style.display = 'none';
        populateTenantSelect();
        populateParentSelect(null);
    }

    function populateTenantSelect() {
        fTenant.innerHTML = '<option value="">— Selecione —</option>';
        tenants.filter(function (t) { return t.active; }).forEach(function (t) {
            var opt = document.createElement('option');
            opt.value = t.id;
            opt.textContent = t.name;
            fTenant.appendChild(opt);
        });
    }

    function populateParentSelect(excludeId) {
        fParent.innerHTML = '<option value="">— Nenhuma (área raiz) —</option>';
        var roots = getRoots();
        roots.forEach(function (root) {
            if (root.id === excludeId) return;
            addParentOption(root, '', excludeId);
        });

        if (preselectedParentId) {
            fParent.value = preselectedParentId;
            updateHierarchyPreview();
        }
    }

    function addParentOption(area, prefix, excludeId) {
        if (area.id === excludeId) return;
        if (excludeId && isDescendant(area.id, excludeId)) return;

        var opt = document.createElement('option');
        opt.value = area.id;
        opt.textContent = prefix + area.name;
        fParent.appendChild(opt);

        var children = getChildren(area.id);
        children.forEach(function (child) {
            addParentOption(child, prefix + '— ', excludeId);
        });
    }

    function isDescendant(areaId, potentialAncestorId) {
        var children = getChildren(potentialAncestorId);
        for (var i = 0; i < children.length; i++) {
            if (children[i].id === areaId) return true;
            if (isDescendant(areaId, children[i].id)) return true;
        }
        return false;
    }

    function updateHierarchyPreview() {
        var parentId = fParent.value || null;
        if (parentId) {
            var path = getAncestorPath(parentId);
            var newName = fName.value.trim() || 'Nova Área';
            path.push(newName);
            hierarchyPath.textContent = path.join(' > ');
            hierarchyPreview.style.display = '';
        } else {
            hierarchyPreview.style.display = 'none';
        }
    }

    function openAddModal(parentId) {
        resetModal();
        preselectedParentId = parentId || null;

        // Auto-select tenant from parent
        if (parentId) {
            var parent = areas.find(function (a) { return a.id === parentId; });
            if (parent && parent.tenantId) {
                // will set after tenant select is populated
                setTimeout(function () {
                    fTenant.value = parent.tenantId;
                    fParent.value = parentId;
                    updateHierarchyPreview();
                }, 0);
            }
        }

        populateParentSelect(null);
        modalTitle.textContent = 'Nova Área';
        modalSubtitle.textContent = 'Preencha os dados abaixo para criar uma área.';
        btnSaveText.textContent = 'Cadastrar';
        $('#areaModal').modal('show');
    }

    function openEditModal(id) {
        resetModal();
        var area = areas.find(function (a) { return a.id === id; });
        if (!area) return;

        editingId = id;
        fName.value = area.name;
        fDesc.value = area.description || '';
        populateParentSelect(id);

        setTimeout(function () {
            fTenant.value = area.tenantId || '';
            fParent.value = area.parentId || '';
            updateHierarchyPreview();
        }, 0);

        modalTitle.textContent = 'Editar Área';
        modalSubtitle.textContent = 'Altere os dados da área.';
        btnSaveText.textContent = 'Salvar Alterações';
        $('#areaModal').modal('show');
    }

    function openDeleteModal(id) {
        var area = areas.find(function (a) { return a.id === id; });
        if (!area) return;
        deletingId = id;
        deleteNameLabel.textContent = '"' + area.name + '"';
        $('#deleteModal').modal('show');
    }

    // ---- Save ----
    async function handleSave() {
        modalAlert.innerHTML = '';

        var name = fName.value.trim();
        if (!name) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> Informe o nome da área.</div>';
            return;
        }

        var tenantId = fTenant.value;
        if (!tenantId) {
            modalAlert.innerHTML = '<div class="alert alert-danger"><i class="la la-warning"></i> Selecione uma empresa.</div>';
            return;
        }

        var parentId = fParent.value || null;

        var payload = {
            name: name,
            description: fDesc.value.trim(),
            parentId: parentId,
            tenantId: tenantId,
            active: true
        };

        btnSave.disabled = true;
        btnSaveText.textContent = 'Salvando...';

        try {
            var url, method;
            if (editingId) {
                url = '/bff/areas/' + editingId;
                method = 'PUT';
            } else {
                url = '/bff/areas';
                method = 'POST';
            }

            var resp = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (resp.ok) {
                $('#areaModal').modal('hide');
                showAlert(editingId ? 'Área atualizada com sucesso.' : 'Área criada com sucesso.', 'success');
                await loadAreas();
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

    // ---- Toggle Active ----
    async function toggleActive(area) {
        try {
            var resp = await fetch('/bff/areas/' + area.id + '/toggle-active', { method: 'PATCH' });
            if (resp.ok) {
                showAlert('Área "' + area.name + '" ' + (area.active ? 'inativada' : 'ativada') + ' com sucesso.', 'success');
                await loadAreas();
            } else {
                showAlert('Erro ao alterar status da área.', 'danger');
            }
        } catch (e) {
            showAlert('Erro de conexão.', 'danger');
        }
    }

    // ---- Delete ----
    async function handleDelete() {
        if (!deletingId) return;
        btnConfirmDelete.disabled = true;

        try {
            var resp = await fetch('/bff/areas/' + deletingId, { method: 'DELETE' });
            if (resp.ok) {
                $('#deleteModal').modal('hide');
                showAlert('Área excluída com sucesso.', 'success');
                await loadAreas();
            } else {
                var body = {};
                try { body = await resp.json(); } catch (e) { }
                showAlert(body.error || 'Ocorreu um erro ao excluir a área.', 'danger');
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
    btnAddRoot.addEventListener('click', function () { openAddModal(null); });
    btnSave.addEventListener('click', handleSave);
    btnConfirmDelete.addEventListener('click', handleDelete);

    fParent.addEventListener('change', updateHierarchyPreview);
    fName.addEventListener('input', function () {
        if (fParent.value) updateHierarchyPreview();
    });

    // ---- Init ----
    (async function init() {
        try {
            await Promise.all([loadAreas(), loadTenants()]);
        } catch (e) {
            showAlert('Erro ao carregar dados iniciais. Recarregue a página.', 'danger');
        }
    })();
})();
