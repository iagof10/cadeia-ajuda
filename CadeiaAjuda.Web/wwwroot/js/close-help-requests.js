(function () {
    'use strict';

    // ---- DOM ----
    var loadingArea = document.getElementById('loadingArea');
    var emptyArea = document.getElementById('emptyArea');
    var tableArea = document.getElementById('tableArea');
    var listBody = document.getElementById('closeListBody');
    var alertArea = document.getElementById('closeAlertArea');

    // ---- Helpers ----
    function escapeHtml(str) {
        var div = document.createElement('div');
        div.textContent = str || '';
        return div.innerHTML;
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
        if (!isoStr) return '\u2014';
        var d = new Date(isoStr);
        return d.toLocaleDateString('pt-BR') + ' ' + d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    }

    function showAlert(msg, type) {
        alertArea.innerHTML =
            '<div class="alert alert-' + type + ' alert-dismissible mb-2" role="alert">' +
            '<button type="button" class="close" data-dismiss="alert"><span>&times;</span></button>' +
            '<i class="la la-' + (type === 'success' ? 'check-circle' : 'warning') + '"></i> ' + escapeHtml(msg) +
            '</div>';
    }

    // ---- API ----
    async function loadRequests() {
        loadingArea.style.display = '';
        emptyArea.style.display = 'none';
        tableArea.style.display = 'none';

        try {
            var resp = await fetch('/bff/help-requests');
            if (!resp.ok) throw new Error('Erro');
            var all = await resp.json();

            // Filter only open requests (status <= 2)
            var open = all.filter(function (r) { return r.status <= 2; });

            loadingArea.style.display = 'none';

            if (open.length === 0) {
                emptyArea.style.display = '';
                return;
            }

            tableArea.style.display = '';
            renderList(open);
        } catch (e) {
            loadingArea.style.display = 'none';
            showAlert('Erro ao carregar chamados.', 'danger');
        }
    }

    function renderList(requests) {
        listBody.innerHTML = '';

        requests.forEach(function (r) {
            var tr = document.createElement('tr');
            tr.innerHTML =
                '<td><span style="font-family:\'Courier New\',monospace;font-weight:700;color:#1e9ff2;">' + escapeHtml(r.code) + '</span></td>' +
                '<td><span style="display:inline-block;width:14px;height:14px;border-radius:50%;background-color:' + escapeHtml(r.sectorColor || '#999') + ';vertical-align:middle;margin-right:6px;border:2px solid rgba(0,0,0,.1);"></span>' + escapeHtml(r.sectorName) + '</td>' +
                '<td>' + escapeHtml(r.helpRequestTypeName) + '</td>' +
                '<td>' + escapeHtml(r.areaName) + '</td>' +
                '<td>' + escapeHtml(r.requestedByUserName) + '</td>' +
                '<td><span class="badge ' + statusClass(r.status) + '" style="font-size:.8rem;padding:4px 10px;border-radius:4px;">' + escapeHtml(r.statusName) + '</span></td>' +
                '<td>' + formatDate(r.createdAt) + '</td>' +
                '<td><button class="btn btn-sm btn-danger btn-close-req" data-id="' + r.id + '" data-code="' + escapeHtml(r.code) + '"><i class="la la-times-circle"></i> Encerrar</button></td>';
            listBody.appendChild(tr);
        });

        // Bind close buttons
        listBody.querySelectorAll('.btn-close-req').forEach(function (btn) {
            btn.addEventListener('click', function () {
                closeRequest(btn);
            });
        });
    }

    async function closeRequest(btn) {
        var id = btn.getAttribute('data-id');
        var code = btn.getAttribute('data-code');

        btn.disabled = true;
        btn.innerHTML = '<i class="la la-spinner la-spin"></i> Encerrando...';

        try {
            var resp = await fetch('/bff/help-requests/' + id + '/close', {
                method: 'PATCH'
            });

            if (resp.ok) {
                showAlert('Chamado ' + code + ' encerrado com sucesso!', 'success');
                await loadRequests();
            } else {
                var body = {};
                try { body = await resp.json(); } catch (e) { }
                showAlert(body.error || 'Erro ao encerrar o chamado.', 'danger');
                btn.disabled = false;
                btn.innerHTML = '<i class="la la-times-circle"></i> Encerrar';
            }
        } catch (e) {
            showAlert('Erro de conexão.', 'danger');
            btn.disabled = false;
            btn.innerHTML = '<i class="la la-times-circle"></i> Encerrar';
        }
    }

    // ---- Init ----
    loadRequests();
})();
