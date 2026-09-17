// dragDropGlobal.js
// This script fixes the file drag and drop issue in WebView2 / MAUI Blazor Hybrid
// by manually intercepting dragover and drop events on file inputs.

document.addEventListener("DOMContentLoaded", () => {
    document.addEventListener("dragover", (e) => {
        if (e.target && e.target.tagName === 'INPUT' && e.target.type === 'file') {
            e.preventDefault();
            e.stopPropagation();
            e.dataTransfer.dropEffect = 'copy';
        }
    });

    document.addEventListener("drop", (e) => {
        if (e.target && e.target.tagName === 'INPUT' && e.target.type === 'file') {
            e.preventDefault();
            e.stopPropagation();
            
            if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
                e.target.files = e.dataTransfer.files;
                const event = new Event("change", { bubbles: true });
                e.target.dispatchEvent(event);
            }
        }
    });

    // Global ESC key listener for Gallery actions
    window.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            // Do not intercept if full-screen image viewer or confirmation dialog is active
            if (document.querySelector(".image-viewer-overlay") || document.querySelector(".dialog-overlay")) {
                return;
            }

            // Close right-click expanded card action backdrop if open
            const backdrop = document.querySelector(".card-action-backdrop");
            if (backdrop) {
                backdrop.click();
                return;
            }

            // Cancel gallery multi-selection mode if active
            const cancelBtn = document.getElementById("gallery-cancel-multiselect-btn");
            if (cancelBtn) {
                cancelBtn.click();
            }
        }
    });
});

// ==========================================================================
// Gallery Scroll Memory per Folder
// ==========================================================================
window.galleryScrollPositions = window.galleryScrollPositions || new Map();

function normalizeFolderPath(p) {
    return (p || '').replace(/\\/g, '/').replace(/\/+$/, '').toLowerCase();
}

// Continuously record scroll position on the gallery container
document.addEventListener('scroll', (e) => {
    const el = e.target;
    if (el && el.id === 'gallery-scroll-container') {
        const folder = el.getAttribute('data-folder');
        if (folder) {
            window.galleryScrollPositions.set(normalizeFolderPath(folder), el.scrollTop);
        }
    }
}, true);

window.saveGalleryScroll = function (folder) {
    const el = document.getElementById('gallery-scroll-container');
    if (el && folder) {
        window.galleryScrollPositions.set(normalizeFolderPath(folder), el.scrollTop);
    }
};

window.restoreGalleryScroll = function (folder) {
    const el = document.getElementById('gallery-scroll-container');
    if (!el || !folder) return;
    const key = normalizeFolderPath(folder);
    const targetY = window.galleryScrollPositions.get(key) || 0;

    el.scrollTop = targetY;
    requestAnimationFrame(() => {
        if (el) el.scrollTop = targetY;
    });
    setTimeout(() => {
        if (el) el.scrollTop = targetY;
    }, 40);
    setTimeout(() => {
        if (el) el.scrollTop = targetY;
    }, 100);
    setTimeout(() => {
        if (el) el.scrollTop = targetY;
    }, 250);
};

window.clearGalleryScroll = function () {
    window.galleryScrollPositions.clear();
};

