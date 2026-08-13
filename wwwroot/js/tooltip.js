document.addEventListener("DOMContentLoaded", () => {
    const tooltipEl = document.createElement('div');
    tooltipEl.className = 'vrc-custom-tooltip';
    document.body.appendChild(tooltipEl);

    let activeTarget = null;

    document.addEventListener("mouseover", (e) => {
        const target = e.target.closest('[title], [data-tooltip]');
        if (target) {
            // Move title attribute to data-tooltip to avoid native OS tooltips
            if (target.hasAttribute('title')) {
                const titleText = target.getAttribute('title');
                if (titleText.trim() !== "") {
                    target.setAttribute('data-tooltip', titleText);
                }
                target.removeAttribute('title');
            }
            
            const tooltipText = target.getAttribute('data-tooltip');
            if (tooltipText && tooltipText.trim() !== "") {
                activeTarget = target;
                tooltipEl.textContent = tooltipText;
                
                // Measure dimensions
                tooltipEl.style.display = 'block';
                const rect = target.getBoundingClientRect();
                const tooltipRect = tooltipEl.getBoundingClientRect();
                tooltipEl.style.display = '';
                
                // Calculate position (default: above the element)
                let top = rect.top - tooltipRect.height - 10;
                let left = rect.left + (rect.width / 2) - (tooltipRect.width / 2);
                
                // Flip to bottom if not enough space at the top
                if (top < 10) {
                    top = rect.bottom + 10;
                    // Add small animation tweak class
                    tooltipEl.style.transformOrigin = 'top center';
                } else {
                    tooltipEl.style.transformOrigin = 'bottom center';
                }
                
                // Keep within horizontal bounds
                if (left < 10) left = 10;
                if (left + tooltipRect.width > window.innerWidth - 10) {
                    left = window.innerWidth - tooltipRect.width - 10;
                }
                
                tooltipEl.style.top = `${top}px`;
                tooltipEl.style.left = `${left}px`;
                tooltipEl.classList.add('visible');
            }
        }
    });

    document.addEventListener("mouseout", (e) => {
        if (activeTarget) {
            // Check if we are really moving out of the target
            const toElement = e.relatedTarget;
            if (!activeTarget.contains(toElement)) {
                tooltipEl.classList.remove('visible');
                activeTarget = null;
            }
        }
    });
    
    // Hide tooltip on click to avoid it sticking around during navigation/actions
    document.addEventListener("mousedown", () => {
        tooltipEl.classList.remove('visible');
        activeTarget = null;
    });

    // Handle scroll events to hide tooltip
    document.addEventListener("scroll", () => {
        if (activeTarget) {
            tooltipEl.classList.remove('visible');
            activeTarget = null;
        }
    }, true);
});
