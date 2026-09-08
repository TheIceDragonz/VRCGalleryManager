window.imageEditor = {
    canvas: null,
    ctx: null,
    dotNetRef: null,
    isDragging: false,
    isColorPicking: false,
    colorPickTarget: null,
    isPinching: false,
    pinchStartDistance: 0,
    pinchStartZoom: 1,
    dragStartX: 0,
    dragStartY: 0,
    width: 0,
    height: 0,
    baseImg: null,
    zoom: 1.0,
    panX: 0,
    panY: 0,
    rotation: 0,
    adaptationMode: 0,
    bgColor: 'transparent',
    syncTimer: null,

    _boundOnMouseDown: null,
    _boundOnMouseMove: null,
    _boundOnMouseUp: null,
    _boundOnTouchStart: null,
    _boundOnTouchMove: null,
    _boundOnTouchEnd: null,
    _boundOnWheel: null,
    _boundOnKeyDown: null,

    init: function (canvasId, dotNetRef) {
        this.removeEventListeners();
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) return;
        this.ctx = this.canvas.getContext('2d', { alpha: true, willReadFrequently: true });
        this.dotNetRef = dotNetRef;
        this.width = this.canvas.width;
        this.height = this.canvas.height;
        this.isColorPicking = false;
        this.isDragging = false;
        this.isPinching = false;

        this._boundOnMouseDown = this.onMouseDown.bind(this);
        this._boundOnMouseMove = this.onMouseMove.bind(this);
        this._boundOnMouseUp = this.onMouseUp.bind(this);
        this._boundOnTouchStart = this.onTouchStart.bind(this);
        this._boundOnTouchMove = this.onTouchMove.bind(this);
        this._boundOnTouchEnd = this.onTouchEnd.bind(this);
        this._boundOnWheel = this.onWheel.bind(this);
        this._boundOnKeyDown = this.onKeyDown.bind(this);

        this.canvas.addEventListener('mousedown', this._boundOnMouseDown);
        window.addEventListener('mousemove', this._boundOnMouseMove, { passive: false });
        window.addEventListener('mouseup', this._boundOnMouseUp);
        
        this.canvas.addEventListener('touchstart', this._boundOnTouchStart, { passive: false });
        window.addEventListener('touchmove', this._boundOnTouchMove, { passive: false });
        window.addEventListener('touchend', this._boundOnTouchEnd);
        window.addEventListener('touchcancel', this._boundOnTouchEnd);

        this.canvas.addEventListener('wheel', this._boundOnWheel, { passive: false });
        window.addEventListener('keydown', this._boundOnKeyDown);
        this.canvas.style.touchAction = 'none';
    },

    removeEventListeners: function () {
        if (this.canvas) {
            if (this._boundOnMouseDown) this.canvas.removeEventListener('mousedown', this._boundOnMouseDown);
            if (this._boundOnTouchStart) this.canvas.removeEventListener('touchstart', this._boundOnTouchStart);
            if (this._boundOnWheel) this.canvas.removeEventListener('wheel', this._boundOnWheel);
            this.canvas.classList.remove('picking-color', 'grabbing');
        }
        if (this._boundOnMouseMove) window.removeEventListener('mousemove', this._boundOnMouseMove);
        if (this._boundOnMouseUp) window.removeEventListener('mouseup', this._boundOnMouseUp);
        if (this._boundOnTouchMove) window.removeEventListener('touchmove', this._boundOnTouchMove);
        if (this._boundOnTouchEnd) {
            window.removeEventListener('touchend', this._boundOnTouchEnd);
            window.removeEventListener('touchcancel', this._boundOnTouchEnd);
        }
        if (this._boundOnKeyDown) window.removeEventListener('keydown', this._boundOnKeyDown);
        document.body.classList.remove('global-grabbing');
    },

    destroy: function () {
        this.stopColorPicker(false);
        if (window.vrcColorPicker) window.vrcColorPicker.close();
        this.removeEventListeners();
        this.canvas = null;
        this.ctx = null;
        this.dotNetRef = null;
        this.baseImg = null;
    },

    loadImagePreview: function (base64Image) {
        if (!this.ctx) return;
        let img = new Image();
        img.onload = () => {
            this.baseImg = img;
            this.draw();
        };
        img.src = base64Image;
    },

    setTransform: function (zoom, panX, panY, rotation, adaptationMode, hexBgColor, isTransparentBg) {
        this.zoom = zoom;
        this.panX = panX;
        this.panY = panY;
        this.rotation = rotation;
        this.adaptationMode = adaptationMode;
        this.bgColor = isTransparentBg ? 'transparent' : hexBgColor;
        this.clampPan();
        this.draw();
    },

    clampPan: function () {
        if (!this.baseImg) return;
        const imgW = this.baseImg.width;
        const imgH = this.baseImg.height;
        if (imgW === 0 || imgH === 0) return;
        
        let rotAngle = this.rotation;
        let isRotated = rotAngle === 90 || rotAngle === 270;
        let actualImgW = isRotated ? imgH : imgW;
        let actualImgH = isRotated ? imgW : imgH;
        
        let baseScale = 1.0;
        if (this.adaptationMode === 0) { // Fit
            baseScale = Math.min(this.width / actualImgW, this.height / actualImgH);
        } else if (this.adaptationMode === 1) { // Fill
            baseScale = Math.max(this.width / actualImgW, this.height / actualImgH);
        } else if (this.adaptationMode === 4) { // Center
            baseScale = 1.0;
        }
        
        let drawnW = 0;
        let drawnH = 0;
        if (this.adaptationMode === 3) { // Stretch
            drawnW = this.width * this.zoom;
            drawnH = this.height * this.zoom;
        } else {
            drawnW = actualImgW * baseScale * this.zoom;
            drawnH = actualImgH * baseScale * this.zoom;
        }

        // Clamp X
        if (drawnW >= this.width) {
            let maxOffset = (drawnW - this.width) / 2;
            this.panX = Math.max(-maxOffset, Math.min(maxOffset, this.panX));
        } else {
            let maxOffset = (this.width - drawnW) / 2;
            this.panX = Math.max(-maxOffset, Math.min(maxOffset, this.panX));
        }

        // Clamp Y
        if (drawnH >= this.height) {
            let maxOffset = (drawnH - this.height) / 2;
            this.panY = Math.max(-maxOffset, Math.min(maxOffset, this.panY));
        } else {
            let maxOffset = (this.height - drawnH) / 2;
            this.panY = Math.max(-maxOffset, Math.min(maxOffset, this.panY));
        }
    },

    draw: function () {
        if (!this.ctx || !this.baseImg) return;
        
        this.ctx.clearRect(0, 0, this.width, this.height);
        
        if (this.bgColor !== 'transparent') {
            this.ctx.fillStyle = this.bgColor;
            this.ctx.fillRect(0, 0, this.width, this.height);
        }
        
        const imgW = this.baseImg.width;
        const imgH = this.baseImg.height;
        if (imgW === 0 || imgH === 0) return;
        
        let rotAngle = this.rotation;
        let isRotated = rotAngle === 90 || rotAngle === 270;
        let actualImgW = isRotated ? imgH : imgW;
        let actualImgH = isRotated ? imgW : imgH;
        
        let baseScale = 1.0;
        if (this.adaptationMode === 0) { // Fit
            baseScale = Math.min(this.width / actualImgW, this.height / actualImgH);
        } else if (this.adaptationMode === 1) { // Fill
            baseScale = Math.max(this.width / actualImgW, this.height / actualImgH);
        } else if (this.adaptationMode === 4) { // Center
            baseScale = 1.0;
        }
        
        this.ctx.save();
        this.ctx.translate(this.width / 2 + this.panX, this.height / 2 + this.panY);
        
        if (this.adaptationMode === 3) { // Stretch
            let scaleX = (this.width / actualImgW) * this.zoom;
            let scaleY = (this.height / actualImgH) * this.zoom;
            this.ctx.scale(scaleX, scaleY);
        } else {
            let finalScale = baseScale * this.zoom;
            this.ctx.scale(finalScale, finalScale);
        }
        
        this.ctx.rotate((rotAngle * Math.PI) / 180);
        this.ctx.drawImage(this.baseImg, -imgW / 2, -imgH / 2);
        this.ctx.restore();
    },

    queueSync: function () {
        clearTimeout(this.syncTimer);
        this.syncTimer = setTimeout(() => {
            if (this.dotNetRef) {
                this.dotNetRef.invokeMethodAsync('OnTransformUpdate', this.zoom, this.panX, this.panY);
            }
        }, 150);
    },

    startColorPicker: function (target, callback = null) {
        this.colorPickTarget = target || 'removeBg';
        this.onColorPickedCallback = callback;
        this.isColorPicking = true;

        if (this.canvas) {
            this.canvas.classList.add('picking-color');
        }
        this.showLoupe();
    },

    showLoupe: function () {
        if (!this.loupeEl) {
            const loupe = document.createElement('div');
            loupe.className = 'vrc-color-loupe';
            loupe.innerHTML = `
                <div class="vrc-loupe-circle">
                    <div class="vrc-loupe-crosshair"></div>
                </div>
                <div class="vrc-loupe-badge">#FFFFFF</div>
            `;
            document.body.appendChild(loupe);
            this.loupeEl = loupe;
        }
        this.loupeEl.style.display = 'none';
    },

    updateLoupe: function (clientX, clientY, hex) {
        if (!this.loupeEl || !this.isColorPicking) return;
        this.loupeEl.style.display = 'flex';
        this.loupeEl.style.left = (clientX + 16) + 'px';
        this.loupeEl.style.top = (clientY - 54) + 'px';
        const circle = this.loupeEl.querySelector('.vrc-loupe-circle');
        const badge = this.loupeEl.querySelector('.vrc-loupe-badge');
        if (circle) circle.style.backgroundColor = hex;
        if (badge) badge.textContent = hex.toUpperCase();
    },

    hideLoupe: function () {
        if (this.loupeEl) {
            this.loupeEl.style.display = 'none';
        }
    },

    stopColorPicker: function (notifyCancel = false) {
        const wasPicking = this.isColorPicking;
        this.isColorPicking = false;
        this.hideLoupe();
        if (this.canvas) {
            this.canvas.classList.remove('picking-color');
        }
        if (this.onColorPickedCallback) {
            if (notifyCancel) this.onColorPickedCallback(null);
            this.onColorPickedCallback = null;
        }
        if (notifyCancel && wasPicking && this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnColorPickCancelled');
        }
    },

    toggleColorPicker: function (target) {
        if (this.isColorPicking) {
            this.stopColorPicker(true);
        } else {
            this.startColorPicker(target);
        }
    },

    onKeyDown: function (e) {
        if (e.key === 'Escape' && this.isColorPicking) {
            this.stopColorPicker(true);
        }
    },

    rgbToHex: function (r, g, b) {
        return ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
    },

    onMouseDown: function (e) {
        if (this.isColorPicking) {
            if (e.button !== undefined && e.button !== 0) {
                // Right or middle click cancels color picking
                this.stopColorPicker(true);
                return;
            }

            const rect = this.canvas.getBoundingClientRect();
            const scaleX = this.canvas.width / rect.width;
            const scaleY = this.canvas.height / rect.height;
            const mouseX = Math.max(0, Math.min(this.canvas.width - 1, Math.floor((e.clientX - rect.left) * scaleX)));
            const mouseY = Math.max(0, Math.min(this.canvas.height - 1, Math.floor((e.clientY - rect.top) * scaleY)));
            
            try {
                const pixel = this.ctx.getImageData(mouseX, mouseY, 1, 1).data;
                const hex = "#" + ("000000" + this.rgbToHex(pixel[0], pixel[1], pixel[2])).slice(-6);
                if (this.onColorPickedCallback) {
                    this.onColorPickedCallback(hex);
                    this.onColorPickedCallback = null;
                }
                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync('OnColorPicked', hex, this.colorPickTarget);
                }
                this.stopColorPicker(false);
            } catch(err) { 
                console.error("Color pick error: ", err); 
                this.stopColorPicker(true);
            }
            return;
        }

        this.isDragging = true;
        this.dragStartX = e.clientX;
        this.dragStartY = e.clientY;
        this.canvas.classList.add('grabbing');
        document.body.classList.add('global-grabbing');
    },

    onMouseMove: function (e) {
        if (this.isColorPicking && this.canvas && this.ctx) {
            const rect = this.canvas.getBoundingClientRect();
            if (e.clientX >= rect.left && e.clientX <= rect.right && e.clientY >= rect.top && e.clientY <= rect.bottom) {
                const scaleX = this.canvas.width / rect.width;
                const scaleY = this.canvas.height / rect.height;
                const mouseX = Math.max(0, Math.min(this.canvas.width - 1, Math.floor((e.clientX - rect.left) * scaleX)));
                const mouseY = Math.max(0, Math.min(this.canvas.height - 1, Math.floor((e.clientY - rect.top) * scaleY)));
                try {
                    const pixel = this.ctx.getImageData(mouseX, mouseY, 1, 1).data;
                    const hex = "#" + ("000000" + this.rgbToHex(pixel[0], pixel[1], pixel[2])).slice(-6);
                    this.updateLoupe(e.clientX, e.clientY, hex);
                } catch (err) {}
            } else {
                this.hideLoupe();
            }
            return;
        }

        if (!this.isDragging) return;
        const dx = e.clientX - this.dragStartX;
        const dy = e.clientY - this.dragStartY;
        this.dragStartX = e.clientX;
        this.dragStartY = e.clientY;
        
        const rect = this.canvas.getBoundingClientRect();
        const scaleX = this.canvas.width / rect.width;
        const scaleY = this.canvas.height / rect.height;
        
        this.panX += dx * scaleX;
        this.panY += dy * scaleY;
        this.clampPan();
        this.draw();
        this.queueSync();
    },

    onMouseUp: function () {
        if (!this.isDragging) return;
        this.isDragging = false;
        this.canvas.classList.remove('grabbing');
        document.body.classList.remove('global-grabbing');
        clearTimeout(this.syncTimer);
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnTransformUpdate', this.zoom, this.panX, this.panY);
        }
    },

    getPinchDistance: function(t1, t2) {
        const dx = t1.clientX - t2.clientX;
        const dy = t1.clientY - t2.clientY;
        return Math.sqrt(dx * dx + dy * dy);
    },

    onTouchStart: function(e) {
        if (this.isColorPicking && e.touches.length === 1) {
            if (e.cancelable) e.preventDefault();
            this.lastTouchX = e.touches[0].clientX;
            this.lastTouchY = e.touches[0].clientY;
            this.onMouseMove(e.touches[0]);
            return;
        }

        if (e.touches.length === 1) {
            if (e.cancelable) e.preventDefault();
            this.onMouseDown(e.touches[0]);
        } else if (e.touches.length === 2) {
            if (e.cancelable) e.preventDefault();
            this.isDragging = false; // Cancel drag if pinching
            this.isPinching = true;
            this.pinchStartDistance = this.getPinchDistance(e.touches[0], e.touches[1]);
            this.pinchStartZoom = this.zoom;
        }
    },

    onTouchMove: function(e) {
        if (this.isColorPicking && e.touches.length === 1) {
            if (e.cancelable) e.preventDefault();
            this.lastTouchX = e.touches[0].clientX;
            this.lastTouchY = e.touches[0].clientY;
            this.onMouseMove(e.touches[0]);
            return;
        }

        if (this.isPinching && e.touches.length === 2) {
            if (e.cancelable) e.preventDefault();
            const currentDist = this.getPinchDistance(e.touches[0], e.touches[1]);
            const zoomFactor = currentDist / this.pinchStartDistance;
            let newZoom = Math.max(1.0, Math.min(this.pinchStartZoom * zoomFactor, 5.0));
            this.zoom = newZoom;
            this.clampPan();
            this.draw();
            this.queueSync();
        } else if (this.isDragging && e.touches.length === 1) {
            if (e.cancelable) e.preventDefault();
            this.onMouseMove(e.touches[0]);
        }
    },

    onTouchEnd: function(e) {
        if (this.isColorPicking) {
            if (this.lastTouchX !== undefined && this.lastTouchY !== undefined) {
                this.onMouseDown({
                    clientX: this.lastTouchX,
                    clientY: this.lastTouchY,
                    button: 0
                });
            }
            return;
        }

        if (e.touches.length < 2) {
            this.isPinching = false;
            if (e.touches.length === 1) {
                // If one finger remains, it could turn into a drag, but to avoid jump we reset drag start
                this.dragStartX = e.touches[0].clientX;
                this.dragStartY = e.touches[0].clientY;
                this.isDragging = true;
            }
        }
        if (e.touches.length === 0) {
            this.onMouseUp();
        }
    },

    onWheel: function (e) {
        e.preventDefault();
        
        const rect = this.canvas.getBoundingClientRect();
        const scaleX = this.canvas.width / rect.width;
        const scaleY = this.canvas.height / rect.height;
        const mouseX = (e.clientX - rect.left) * scaleX;
        const mouseY = (e.clientY - rect.top) * scaleY;

        let delta = e.deltaY;
        let zoomFactor = delta > 0 ? 0.9 : 1.1;
        let newZoom = Math.max(1.0, Math.min(this.zoom * zoomFactor, 5.0));
        
        let actualZoomFactor = newZoom / this.zoom;
        
        const centerX = this.width / 2;
        const centerY = this.height / 2;
        
        const dx = mouseX - centerX - this.panX;
        const dy = mouseY - centerY - this.panY;
        
        this.panX = mouseX - centerX - dx * actualZoomFactor;
        this.panY = mouseY - centerY - dy * actualZoomFactor;
        this.zoom = newZoom;

        this.clampPan();
        this.draw();
        this.queueSync();
    }
};

window.spritesheetAnimator = {
    animators: [],
    loopActive: false,

    register: function (element) {
        if (!element) return;
        // Check if already registered
        if (this.animators.some(a => a.el === element)) return;

        const frames = parseInt(element.getAttribute('data-frames') || '1');
        const fps = parseInt(element.getAttribute('data-fps') || '15');
        const isAnimated = element.getAttribute('data-animated') === 'true';

        if (!isAnimated || frames < 1) return;

        const cols = frames <= 16 ? 4 : 8;

        this.animators.push({
            el: element,
            frames: frames,
            fps: fps,
            cols: cols,
            currentFrame: -1
        });

        if (!this.loopActive) {
            this.loopActive = true;
            this.startLoop();
        }
    },

    initAll: function () {
        const elements = document.querySelectorAll('.spritesheet-animator');
        elements.forEach(el => {
            this.unregister(el);
            this.register(el);
        });
    },

    unregister: function (element) {
        this.animators = this.animators.filter(a => a.el !== element);
    },

    clear: function () {
        this.animators = [];
        this.loopActive = false;
    },

    startLoop: function () {
        const update = (timestamp) => {
            if (this.animators.length === 0) {
                this.loopActive = false;
                return;
            }

            for (let i = 0; i < this.animators.length; i++) {
                const anim = this.animators[i];
                if (!document.body.contains(anim.el)) {
                    // Element was removed from DOM, unregister
                    this.animators.splice(i, 1);
                    i--;
                    continue;
                }

                // Calculate current frame based on time and FPS
                const msPerFrame = 1000 / anim.fps;
                const totalDuration = msPerFrame * anim.frames;
                const elapsed = timestamp % totalDuration;
                const frame = Math.floor(elapsed / msPerFrame) % anim.frames;

                if (frame !== anim.currentFrame) {
                    anim.currentFrame = frame;
                    const col = frame % anim.cols;
                    const row = Math.floor(frame / anim.cols);

                    // Update background position and size
                    const pctX = anim.cols > 1 ? (col / (anim.cols - 1)) * 100 : 0;
                    const pctY = anim.cols > 1 ? (row / (anim.cols - 1)) * 100 : 0;

                    anim.el.style.backgroundSize = `${anim.cols * 100}% ${anim.cols * 100}%`;
                    anim.el.style.backgroundPosition = `${pctX}% ${pctY}%`;
                }
            }

            requestAnimationFrame(update);
        };
        requestAnimationFrame(update);
    }
};

document.addEventListener('wheel', function(e) {
    let timeline = e.target.closest('.timeline-scroll');
    if (timeline) {
        if (e.deltaY !== 0) {
            timeline.scrollLeft += Math.sign(e.deltaY) * 60;
            e.preventDefault();
        }
    }
}, { passive: false });

window.vrcColorPicker = {
    activeInput: null,
    popoverEl: null,
    currentHue: 0,
    currentSat: 100,
    currentVal: 100,
    isDraggingSV: false,

    init: function () {
        if (this.popoverEl) return;
        this.createDOM();
        this.bindGlobalEvents();
    },

    createDOM: function () {
        const el = document.createElement('div');
        el.id = 'vrcColorPickerPopover';
        el.className = 'vrc-color-picker-popover';
        el.style.display = 'none';

        el.innerHTML = `
            <div class="vrc-picker-header">
                <span class="vrc-picker-title">Color Picker</span>
                <button type="button" class="vrc-picker-close-btn" aria-label="Close">✕</button>
            </div>
            
            <div class="vrc-picker-sv-box" id="vrcPickerSV">
                <div class="vrc-picker-sv-cursor" id="vrcPickerSVCursor"></div>
            </div>

            <div class="vrc-picker-controls-bar">
                <button type="button" class="vrc-picker-eyedropper-btn" id="vrcPickerPipette" aria-label="Pick color from image (Pipette)">
                    <img src="/svg/color-picker-svgrepo-com.svg" class="icon-accent" style="width: 16px; height: 16px;" alt="Pipette" />
                </button>
                <div class="vrc-picker-preview-circle" id="vrcPickerPreview"></div>
                <div class="vrc-picker-hue-container">
                    <input type="range" class="vrc-picker-hue-slider" id="vrcPickerHue" min="0" max="360" value="0">
                </div>
            </div>

            <div class="vrc-picker-inputs-bar">
                <div class="vrc-picker-field hex-field">
                    <label>HEX</label>
                    <input type="text" id="vrcPickerHex" maxlength="7" spellcheck="false" />
                </div>
                <div class="vrc-picker-field rgb-field">
                    <label>R</label>
                    <input type="number" id="vrcPickerR" min="0" max="255" />
                </div>
                <div class="vrc-picker-field rgb-field">
                    <label>G</label>
                    <input type="number" id="vrcPickerG" min="0" max="255" />
                </div>
                <div class="vrc-picker-field rgb-field">
                    <label>B</label>
                    <input type="number" id="vrcPickerB" min="0" max="255" />
                </div>
            </div>

            <div class="vrc-picker-swatches-bar">
                <div class="vrc-picker-swatch" data-color="#ffffff" style="background:#ffffff;" aria-label="White"></div>
                <div class="vrc-picker-swatch" data-color="#000000" style="background:#000000;" aria-label="Black"></div>
                <div class="vrc-picker-swatch" data-color="#6ae3f9" style="background:#6ae3f9;" aria-label="Cyan Accent"></div>
                <div class="vrc-picker-swatch" data-color="#00ff00" style="background:#00ff00;" aria-label="Green Screen"></div>
                <div class="vrc-picker-swatch" data-color="#ff0000" style="background:#ff0000;" aria-label="Red"></div>
                <div class="vrc-picker-swatch" data-color="#0055ff" style="background:#0055ff;" aria-label="Blue"></div>
                <div class="vrc-picker-swatch" data-color="#ffff00" style="background:#ffff00;" aria-label="Yellow"></div>
                <div class="vrc-picker-swatch" data-color="#ff00ff" style="background:#ff00ff;" aria-label="Magenta"></div>
            </div>
        `;

        document.body.appendChild(el);
        this.popoverEl = el;

        this.svBox = el.querySelector('#vrcPickerSV');
        this.svCursor = el.querySelector('#vrcPickerSVCursor');
        this.pipetteBtn = el.querySelector('#vrcPickerPipette');
        this.previewCircle = el.querySelector('#vrcPickerPreview');
        this.hueSlider = el.querySelector('#vrcPickerHue');
        this.hexInput = el.querySelector('#vrcPickerHex');
        this.rInput = el.querySelector('#vrcPickerR');
        this.gInput = el.querySelector('#vrcPickerG');
        this.bInput = el.querySelector('#vrcPickerB');
        this.closeBtn = el.querySelector('.vrc-picker-close-btn');

        this.bindInternalEvents();
    },

    bindGlobalEvents: function () {
        document.addEventListener('click', (e) => {
            const colorInput = e.target.closest('input[type="color"]');
            if (colorInput && !colorInput.disabled) {
                e.preventDefault();
                e.stopPropagation();
                this.open(colorInput);
                return;
            }

            if (this.popoverEl && this.popoverEl.style.display !== 'none') {
                if (!this.popoverEl.contains(e.target)) {
                    this.close();
                }
            }
        }, true);

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.popoverEl && this.popoverEl.style.display !== 'none') {
                this.close();
            }
        });
    },

    bindInternalEvents: function () {
        this.closeBtn.addEventListener('click', () => this.close());

        this.pipetteBtn.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();
            this.startEyedropper();
        });

        this.hueSlider.addEventListener('input', (e) => {
            this.currentHue = parseFloat(e.target.value);
            this.updateSVBoxColor();
            this.applyCurrentColor();
        });

        const updateSVFromEvent = (e) => {
            const rect = this.svBox.getBoundingClientRect();
            const clientX = e.touches ? e.touches[0].clientX : e.clientX;
            const clientY = e.touches ? e.touches[0].clientY : e.clientY;
            let x = Math.max(0, Math.min(rect.width, clientX - rect.left));
            let y = Math.max(0, Math.min(rect.height, clientY - rect.top));
            this.currentSat = (x / rect.width) * 100;
            this.currentVal = (1 - (y / rect.height)) * 100;
            this.updateSVCursorPosition();
            this.applyCurrentColor();
        };

        this.svBox.addEventListener('mousedown', (e) => {
            e.preventDefault();
            this.isDraggingSV = true;
            updateSVFromEvent(e);
        });

        this.svBox.addEventListener('touchstart', (e) => {
            if (e.cancelable) e.preventDefault();
            this.isDraggingSV = true;
            updateSVFromEvent(e);
        }, { passive: false });

        window.addEventListener('mousemove', (e) => {
            if (this.isDraggingSV) {
                e.preventDefault();
                updateSVFromEvent(e);
            }
        });

        window.addEventListener('touchmove', (e) => {
            if (this.isDraggingSV) {
                if (e.cancelable) e.preventDefault();
                updateSVFromEvent(e);
            }
        }, { passive: false });

        window.addEventListener('mouseup', () => {
            if (this.isDraggingSV) {
                this.isDraggingSV = false;
                this.notifyChange();
            }
        });

        window.addEventListener('touchend', () => {
            if (this.isDraggingSV) {
                this.isDraggingSV = false;
                this.notifyChange();
            }
        });

        this.hexInput.addEventListener('change', (e) => {
            let val = e.target.value.trim();
            if (!val.startsWith('#')) val = '#' + val;
            const rgb = this.hexToRgb(val);
            if (rgb) {
                this.setColorFromRgb(rgb.r, rgb.g, rgb.b, true);
            } else {
                this.updateInputFields();
            }
        });

        const onRgbInput = () => {
            let r = parseInt(this.rInput.value) || 0;
            let g = parseInt(this.gInput.value) || 0;
            let b = parseInt(this.bInput.value) || 0;
            r = Math.max(0, Math.min(255, r));
            g = Math.max(0, Math.min(255, g));
            b = Math.max(0, Math.min(255, b));
            this.setColorFromRgb(r, g, b, true);
        };

        this.rInput.addEventListener('change', onRgbInput);
        this.gInput.addEventListener('change', onRgbInput);
        this.bInput.addEventListener('change', onRgbInput);

        this.popoverEl.querySelectorAll('.vrc-picker-swatch').forEach(swatch => {
            swatch.addEventListener('click', (e) => {
                const color = e.target.getAttribute('data-color');
                if (color) {
                    const rgb = this.hexToRgb(color);
                    if (rgb) {
                        this.setColorFromRgb(rgb.r, rgb.g, rgb.b, true);
                    }
                }
            });
        });
    },

    open: function (inputEl) {
        // Dismiss any active tooltip immediately
        const tooltip = document.querySelector('.vrc-custom-tooltip');
        if (tooltip) {
            tooltip.classList.remove('visible');
        }

        this.init();
        this.activeInput = inputEl;

        const rect = inputEl.getBoundingClientRect();
        this.popoverEl.style.display = 'block';

        const popoverWidth = 250;
        const popoverHeight = 310;
        let top = rect.bottom + 8;
        let left = rect.right - popoverWidth;

        if (top + popoverHeight > window.innerHeight) {
            top = Math.max(10, rect.top - popoverHeight - 8);
        }
        if (left < 10) left = 10;
        if (left + popoverWidth > window.innerWidth) {
            left = window.innerWidth - popoverWidth - 10;
        }

        this.popoverEl.style.top = top + 'px';
        this.popoverEl.style.left = left + 'px';

        const initialHex = inputEl.value || '#ffffff';
        const rgb = this.hexToRgb(initialHex) || { r: 255, g: 255, b: 255 };
        this.setColorFromRgb(rgb.r, rgb.g, rgb.b, false);
    },

    close: function () {
        if (this.popoverEl) {
            this.popoverEl.style.display = 'none';
        }
        this.activeInput = null;
    },

    setColorFromRgb: function (r, g, b, notify = true) {
        const hsv = this.rgbToHsv(r, g, b);
        this.currentHue = hsv.h;
        this.currentSat = hsv.s;
        this.currentVal = hsv.v;

        this.hueSlider.value = this.currentHue;
        this.updateSVBoxColor();
        this.updateSVCursorPosition();
        this.updateUI(r, g, b);

        if (notify) {
            this.applyCurrentColor();
            this.notifyChange();
        }
    },

    updateSVBoxColor: function () {
        this.svBox.style.background = `linear-gradient(to bottom, transparent, #000000), linear-gradient(to right, #ffffff, hsl(${this.currentHue}, 100%, 50%))`;
    },

    updateSVCursorPosition: function () {
        const rect = this.svBox.getBoundingClientRect();
        const w = rect.width || 224;
        const h = rect.height || 130;
        const x = (this.currentSat / 100) * w;
        const y = (1 - (this.currentVal / 100)) * h;
        this.svCursor.style.left = x + 'px';
        this.svCursor.style.top = y + 'px';
    },

    applyCurrentColor: function () {
        const rgb = this.hsvToRgb(this.currentHue, this.currentSat, this.currentVal);
        const hex = this.rgbToHex(rgb.r, rgb.g, rgb.b);
        this.updateUI(rgb.r, rgb.g, rgb.b);

        if (this.activeInput) {
            this.activeInput.value = hex;
            this.activeInput.dispatchEvent(new Event('input', { bubbles: true }));
        }
    },

    notifyChange: function () {
        if (this.activeInput) {
            this.activeInput.dispatchEvent(new Event('change', { bubbles: true }));
        }
    },

    updateUI: function (r, g, b) {
        const hex = this.rgbToHex(r, g, b);
        this.previewCircle.style.backgroundColor = hex;
        this.hexInput.value = hex.toUpperCase();
        this.rInput.value = r;
        this.gInput.value = g;
        this.bInput.value = b;
    },

    updateInputFields: function () {
        const rgb = this.hsvToRgb(this.currentHue, this.currentSat, this.currentVal);
        this.updateUI(rgb.r, rgb.g, rgb.b);
    },

    startEyedropper: function () {
        const targetInput = this.activeInput;
        if (!targetInput) return;

        let targetType = 'removeBg';
        const parentSection = targetInput.closest('.mb-3') || targetInput.closest('.card-body');
        if (parentSection && parentSection.textContent.toLowerCase().includes('background color')) {
            targetType = 'bgColor';
        }

        this.popoverEl.style.display = 'none';

        if (window.imageEditor) {
            window.imageEditor.startColorPicker(targetType, (pickedHex) => {
                if (pickedHex) {
                    targetInput.value = pickedHex;
                    targetInput.dispatchEvent(new Event('input', { bubbles: true }));
                    targetInput.dispatchEvent(new Event('change', { bubbles: true }));
                    this.open(targetInput);
                }
            });
        }
    },

    rgbToHsv: function (r, g, b) {
        r /= 255; g /= 255; b /= 255;
        let max = Math.max(r, g, b), min = Math.min(r, g, b);
        let h, s, v = max;
        let d = max - min;
        s = max === 0 ? 0 : d / max;
        if (max === min) {
            h = 0;
        } else {
            switch (max) {
                case r: h = (g - b) / d + (g < b ? 6 : 0); break;
                case g: h = (b - r) / d + 2; break;
                case b: h = (r - g) / d + 4; break;
            }
            h /= 6;
        }
        return { h: Math.round(h * 360), s: Math.round(s * 100), v: Math.round(v * 100) };
    },

    hsvToRgb: function (h, s, v) {
        h = (h % 360) / 360;
        s = s / 100;
        v = v / 100;
        let r, g, b;
        let i = Math.floor(h * 6);
        let f = h * 6 - i;
        let p = v * (1 - s);
        let q = v * (1 - f * s);
        let t = v * (1 - (1 - f) * s);
        switch (i % 6) {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }
        return {
            r: Math.round(r * 255),
            g: Math.round(g * 255),
            b: Math.round(b * 255)
        };
    },

    rgbToHex: function (r, g, b) {
        const toHex = (c) => ('0' + Math.max(0, Math.min(255, Math.round(c))).toString(16)).slice(-2);
        return `#${toHex(r)}${toHex(g)}${toHex(b)}`.toLowerCase();
    },

    hexToRgb: function (hex) {
        let clean = hex.replace('#', '');
        if (clean.length === 3) {
            clean = clean.split('').map(c => c + c).join('');
        }
        if (clean.length !== 6) return null;
        let num = parseInt(clean, 16);
        if (isNaN(num)) return null;
        return {
            r: (num >> 16) & 255,
            g: (num >> 8) & 255,
            b: num & 255
        };
    }
};

// Initialize on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => window.vrcColorPicker.init());
} else {
    window.vrcColorPicker.init();
}

