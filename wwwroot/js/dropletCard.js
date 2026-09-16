/**
 * dropletCard.js
 * Calcola dinamicamente le coordinate (bottom, right) dei pulsanti d'azione
 * all'interno delle card droplet (.icon-droplet-card con border-radius: 50% 50% 20px 50%).
 * 
 * Garantisce che ad ogni dimensione della card, risoluzione, zoom o ridimensionamento finestra,
 * i pulsanti seguano con precisione matematica l'arco circolare del bordo inferiore sinistro,
 * mantenendo un margine uniforme e costante di 5px dal bordo senza mai essere tagliati o fluttuare.
 */
(function () {
    const cardObservers = new WeakMap();

    function layoutDropletCard(card) {
        if (!card) return;
        const actions = card.querySelector('.media-card-actions');
        if (!actions) return;

        const W = card.offsetWidth;
        if (W <= 0) return;

        const buttons = actions.querySelectorAll('.btn-action');
        if (buttons.length === 0) return;

        const r = 15.0; // raggio pulsante (diametro 30px / 2)
        const m = 5.0;  // margine dal bordo della card
        const d_edge = r + m; // 20.0px dal bordo esterno
        const R_btn = Math.max(10.0, W / 2.0 - d_edge);
        const L_flat = Math.max(10.0, W / 2.0 - d_edge);
        const L_arc = R_btn * (Math.PI / 2.0);
        const L_total = L_flat + L_arc;

        const maxStep = 34.0; // 30px pulsante + 4px gap standard
        const step = buttons.length > 1 ? Math.min(maxStep, (L_total - 4.0) / (buttons.length - 1)) : maxStep;

        buttons.forEach((btn, i) => {
            const s = i * step;
            let x, y;
            if (s <= L_flat) {
                x = (W - d_edge) - s;
                y = W - d_edge;
            } else {
                const s_arc = s - L_flat;
                const phi = s_arc / R_btn;
                x = (W / 2.0) - R_btn * Math.sin(phi);
                y = (W / 2.0) + R_btn * Math.cos(phi);
            }

            const right = W - x - r;
            const bottom = W - y - r;

            btn.style.setProperty('right', right.toFixed(1) + 'px', 'important');
            btn.style.setProperty('bottom', bottom.toFixed(1) + 'px', 'important');
        });
    }

    const resizeObserver = new ResizeObserver((entries) => {
        for (const entry of entries) {
            layoutDropletCard(entry.target);
        }
    });

    function updateAllDroplets() {
        const dropletCards = document.querySelectorAll('.icon-droplet-card');
        dropletCards.forEach(card => {
            if (!cardObservers.has(card)) {
                cardObservers.set(card, true);
                resizeObserver.observe(card);
            }
            layoutDropletCard(card);
        });
    }

    const mutationObserver = new MutationObserver((mutations) => {
        let hasChanges = false;
        for (const mutation of mutations) {
            if (mutation.type === 'childList') {
                hasChanges = true;
                break;
            }
        }
        if (hasChanges) {
            updateAllDroplets();
            requestAnimationFrame(updateAllDroplets);
        }
    });

    function init() {
        mutationObserver.observe(document.body, { childList: true, subtree: true });
        updateAllDroplets();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Mobile Touch Long-Press Support (~350ms) with movement cancellation and haptic feedback
    let touchStartX = 0;
    let touchStartY = 0;
    let longPressTimer = null;
    let longPressFired = false;
    let lastLongPressTime = 0;

    document.addEventListener('touchstart', (e) => {
        if (e.touches.length !== 1) {
            clearTimeout(longPressTimer);
            longPressTimer = null;
            return;
        }

        const touch = e.touches[0];
        if (touch.target.closest('.media-card-actions') || 
            touch.target.closest('.btn-action') || 
            touch.target.closest('.card-action-backdrop')) {
            return;
        }

        const card = touch.target.closest('.media-card-container');
        if (!card) return;

        touchStartX = touch.clientX;
        touchStartY = touch.clientY;
        longPressFired = false;

        clearTimeout(longPressTimer);
        longPressTimer = setTimeout(() => {
            longPressFired = true;
            lastLongPressTime = Date.now();

            if (navigator.vibrate) {
                try { navigator.vibrate(40); } catch (_) {}
            }

            const contextEvent = new MouseEvent('contextmenu', {
                bubbles: true,
                cancelable: true,
                view: window,
                clientX: touchStartX,
                clientY: touchStartY,
                button: 2
            });
            contextEvent.isCustomLongPress = true;
            (touch.target || card).dispatchEvent(contextEvent);
        }, 350);
    }, { passive: true });

    document.addEventListener('touchmove', (e) => {
        if (!longPressTimer) return;
        if (e.touches.length !== 1) {
            clearTimeout(longPressTimer);
            longPressTimer = null;
            return;
        }
        const touch = e.touches[0];
        const dx = touch.clientX - touchStartX;
        const dy = touch.clientY - touchStartY;
        // 16px threshold prevents micro-tremor cancellations while cancelling cleanly on scroll
        if (Math.hypot(dx, dy) > 16) {
            clearTimeout(longPressTimer);
            longPressTimer = null;
        }
    }, { passive: true });

    document.addEventListener('touchend', (e) => {
        clearTimeout(longPressTimer);
        longPressTimer = null;
        if (longPressFired) {
            e.preventDefault();
            longPressFired = false;
        }
    }, { passive: false });

    document.addEventListener('touchcancel', () => {
        clearTimeout(longPressTimer);
        longPressTimer = null;
        longPressFired = false;
    });

    // Ascolta eventi contestuali o clic per aggiornare all'istante
    document.addEventListener('contextmenu', (e) => {
        clearTimeout(longPressTimer);
        longPressTimer = null;

        // Previeni eventuale evento contestuale nativo duplicato generato dalla WebView dopo il nostro long-press
        if (Date.now() - lastLongPressTime < 450 && !e.isCustomLongPress) {
            e.preventDefault();
            e.stopImmediatePropagation();
            return;
        }

        if (e.target && e.target.closest && e.target.closest('.icon-droplet-card')) {
            requestAnimationFrame(updateAllDroplets);
            setTimeout(updateAllDroplets, 40);
        }
    }, true);

    window.updateDropletCards = updateAllDroplets;
    window.addEventListener('resize', updateAllDroplets, { passive: true });
})();
