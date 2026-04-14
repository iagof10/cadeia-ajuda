(function () {
    'use strict';

    /**
     * Aplica máscara a um campo de input.
     * @param {HTMLInputElement} input
     * @param {string} mask - Ex: '00.000.000/0000-00', '(00) 00000-0000'
     */
    function applyMask(input, mask) {
        input.addEventListener('input', function () {
            var value = input.value.replace(/\D/g, '');
            var result = '';
            var vi = 0;

            for (var mi = 0; mi < mask.length && vi < value.length; mi++) {
                if (mask[mi] === '0') {
                    result += value[vi];
                    vi++;
                } else {
                    result += mask[mi];
                    if (value[vi] === mask[mi]) vi++;
                }
            }

            var nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
            nativeInputValueSetter.call(input, result);
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });
    }

    /**
     * Aplica máscara de telefone (00) 0000-0000 ou (00) 00000-0000
     * @param {HTMLInputElement} input
     */
    function applyPhoneMask(input) {
        input.addEventListener('input', function () {
            var value = input.value.replace(/\D/g, '');
            if (value.length > 11) value = value.substring(0, 11);

            if (value.length <= 10) {
                value = value.replace(/^(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{4})(\d)/, '$1-$2');
            } else {
                value = value.replace(/^(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{5})(\d)/, '$1-$2');
            }

            var nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
            nativeInputValueSetter.call(input, value);
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });
    }

    /**
     * Inicializa máscaras em campos com data-mask
     * Uso: <input data-mask="cnpj" /> ou <input data-mask="phone" />
     */
    function initMasks(root) {
        root = root || document;

        root.querySelectorAll('[data-mask="cnpj"]').forEach(function (el) {
            if (el.__masked) return;
            el.__masked = true;
            el.maxLength = 18;
            applyMask(el, '00.000.000/0000-00');
        });

        root.querySelectorAll('[data-mask="phone"]').forEach(function (el) {
            if (el.__masked) return;
            el.__masked = true;
            el.maxLength = 15;
            applyPhoneMask(el);
        });

        root.querySelectorAll('[data-mask="cpf"]').forEach(function (el) {
            if (el.__masked) return;
            el.__masked = true;
            el.maxLength = 14;
            applyMask(el, '000.000.000-00');
        });
    }

    // Expor globalmente
    window.initInputMasks = initMasks;

    // Auto-inicializar ao carregar
    document.addEventListener('DOMContentLoaded', function () { initMasks(); });

    // Observer para detectar quando Blazor re-renderiza o DOM
    var observer = new MutationObserver(function () { initMasks(); });
    observer.observe(document.body, { childList: true, subtree: true });
})();
