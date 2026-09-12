window.imageViewerZoom = {
    init: function (containerElement) {
        if (!containerElement) return;

        // Prevent double init
        if (containerElement._isZoomInitialized) return;
        containerElement._isZoomInitialized = true;

        let scale = 1;
        let pointX = 0;
        let pointY = 0;
        
        let pointers = []; 
        let initialDistance = 0;
        let initialScale = 1;
        
        let start = { x: 0, y: 0 };
        let isPanning = false;
        let hasDragged = false;
        let startPoint = { x: 0, y: 0 };

        function enforceBounds(isPointerMove = false, e = null) {
            const img = containerElement.querySelector('.viewer-img.opacity-100') || containerElement.querySelector('.viewer-img');
            if (!img) return;

            const w = img.clientWidth * scale;
            const h = img.clientHeight * scale;
            
            const baseLeft = img.offsetLeft;
            const baseTop = img.offsetTop;
            
            const marginX = 100;
            const marginY = 100;
            
            const maxPointX = window.innerWidth - marginX - baseLeft;
            const minPointX = marginX - baseLeft - w;
            
            const maxPointY = window.innerHeight - marginY - baseTop;
            const minPointY = marginY - baseTop - h;
            
            const clampedX = Math.max(minPointX, Math.min(pointX, maxPointX));
            const clampedY = Math.max(minPointY, Math.min(pointY, maxPointY));
            
            if (isPointerMove && e) {
                if (clampedX !== pointX) start.x = e.clientX - clampedX;
                if (clampedY !== pointY) start.y = e.clientY - clampedY;
            }
            
            pointX = clampedX;
            pointY = clampedY;
        }

        function setTransform() {
            const imgs = containerElement.querySelectorAll('.viewer-img');
            imgs.forEach(i => {
                i.style.transformOrigin = '0 0';
                i.style.transform = `translate(${pointX}px, ${pointY}px) scale(${scale})`;
            });
        }

        containerElement.addEventListener('pointerdown', (e) => {
            if (e.target.closest('.btn') || e.target.closest('.image-viewer-top-bar')) return;

            hasDragged = false;
            startPoint = { x: e.clientX, y: e.clientY };

            pointers.push(e);
            
            if (pointers.length === 1) {
                isPanning = true;
                start = { x: e.clientX - pointX, y: e.clientY - pointY };
                
                const imgs = containerElement.querySelectorAll('.viewer-img');
                imgs.forEach(i => i.style.transition = "none");
                
                containerElement.setPointerCapture(e.pointerId);
            } 
            else if (pointers.length === 2) {
                isPanning = false; 
                initialDistance = Math.hypot(
                    pointers[0].clientX - pointers[1].clientX,
                    pointers[0].clientY - pointers[1].clientY
                );
                initialScale = scale;
            }
        });

        // Prevent native image dragging which interrupts pointer events on left-click
        containerElement.addEventListener('dragstart', (e) => {
            e.preventDefault();
        });

        containerElement.addEventListener('pointermove', (e) => {
            if (Math.hypot(e.clientX - startPoint.x, e.clientY - startPoint.y) > 5) {
                hasDragged = true;
            }

            const index = pointers.findIndex(p => p.pointerId === e.pointerId);
            if (index !== -1) {
                pointers[index] = e;
            }

            if (pointers.length === 1 && isPanning) {
                e.preventDefault();
                pointX = (e.clientX - start.x);
                pointY = (e.clientY - start.y);
                enforceBounds(true, e);
                setTransform();
            } 
            else if (pointers.length === 2) {
                e.preventDefault();
                const currentDistance = Math.hypot(
                    pointers[0].clientX - pointers[1].clientX,
                    pointers[0].clientY - pointers[1].clientY
                );
                
                const centerX = (pointers[0].clientX + pointers[1].clientX) / 2;
                const centerY = (pointers[0].clientY + pointers[1].clientY) / 2;
                
                let targetScale = initialScale * (currentDistance / initialDistance);
                targetScale = Math.max(0.2, Math.min(targetScale, 15));
                
                let ratio = targetScale / scale;
                
                const img = containerElement.querySelector('.viewer-img.opacity-100') || containerElement.querySelector('.viewer-img');
                if (img) {
                    const rect = img.getBoundingClientRect();
                    let mouseX = centerX - rect.left;
                    let mouseY = centerY - rect.top;
                    
                    pointX -= (mouseX * ratio - mouseX);
                    pointY -= (mouseY * ratio - mouseY);
                }
                
                scale = targetScale;
                enforceBounds();
                setTransform();
            }
        });

        const pointerUpHandler = (e) => {
            pointers = pointers.filter(p => p.pointerId !== e.pointerId);
            if (pointers.length < 2) {
                if (pointers.length === 1) {
                    start = { x: pointers[0].clientX - pointX, y: pointers[0].clientY - pointY };
                    isPanning = true;
                } else {
                    isPanning = false;
                    const imgs = containerElement.querySelectorAll('.viewer-img');
                    imgs.forEach(i => i.style.transition = "opacity 0.3s");
                }
            }
        };

        containerElement.addEventListener('pointerup', pointerUpHandler);
        containerElement.addEventListener('pointercancel', pointerUpHandler);
        
        containerElement.addEventListener('wheel', (e) => {
            e.preventDefault();
            
            const imgs = containerElement.querySelectorAll('.viewer-img');
            imgs.forEach(i => i.style.transition = "none");
            
            let delta = (e.wheelDelta ? e.wheelDelta : -e.deltaY);
            let newScale = scale * (delta > 0 ? 1.15 : 1 / 1.15);
            newScale = Math.max(0.2, Math.min(newScale, 15));
            
            let ratio = newScale / scale;
            
            const img = containerElement.querySelector('.viewer-img.opacity-100') || containerElement.querySelector('.viewer-img');
            if (img) {
                const rect = img.getBoundingClientRect();
                let mouseX = e.clientX - rect.left;
                let mouseY = e.clientY - rect.top;
                
                pointX -= (mouseX * ratio - mouseX);
                pointY -= (mouseY * ratio - mouseY);
            }
            
            scale = newScale;
            enforceBounds();
            setTransform();
            
            clearTimeout(containerElement._wheelTimeout);
            containerElement._wheelTimeout = setTimeout(() => {
                imgs.forEach(i => i.style.transition = "opacity 0.3s");
            }, 100);
            
        }, { passive: false });

        containerElement.addEventListener('click', (e) => {
            if (hasDragged) {
                e.stopPropagation();
                // We reset it here so next click works
                hasDragged = false;
                return;
            }

            // Because of pointer capture, clicks on the image might have the container as target.
            // We use elementFromPoint to check if the user actually clicked the image.
            const el = document.elementFromPoint(e.clientX, e.clientY);
            if (el && el.classList.contains('viewer-img')) {
                e.stopPropagation();
            }
        }, true);

        containerElement._resetZoom = function() {
            scale = 1; pointX = 0; pointY = 0;
            const imgs = containerElement.querySelectorAll('.viewer-img');
            imgs.forEach(i => {
                i.style.transition = "transform 0.2s cubic-bezier(0.175, 0.885, 0.32, 1.275), opacity 0.3s";
                i.style.transformOrigin = 'center center'; // reset origin for animation
                i.style.transform = `translate(0px, 0px) scale(1)`;
            });
            setTimeout(() => {
                imgs.forEach(i => {
                    i.style.transition = "opacity 0.3s";
                    i.style.transformOrigin = '0 0'; // prepare for next zoom
                });
            }, 250);
        };
    },
    reset: function(containerElement) {
        if (containerElement && containerElement._resetZoom) {
            containerElement._resetZoom();
        }
    },
    getDimensions: function(containerElement) {
        if (!containerElement) return [0, 0];
        const img = containerElement.querySelector('.viewer-img.opacity-100') || containerElement.querySelector('.viewer-img');
        if (img && img.naturalWidth && img.naturalHeight) {
            return [img.naturalWidth, img.naturalHeight];
        }
        return [0, 0];
    }
};
