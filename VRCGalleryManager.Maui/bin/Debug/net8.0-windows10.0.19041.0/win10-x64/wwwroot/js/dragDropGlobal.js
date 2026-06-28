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
});
