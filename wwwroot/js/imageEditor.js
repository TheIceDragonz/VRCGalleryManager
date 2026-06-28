window.imageEditor = {
    canvas: null,
    ctx: null,
    dotNetRef: null,
    isDragging: false,
    isColorPicking: false,
    dragStartX: 0,
    dragStartY: 0,
    width: 0,
    baseImg: null,
    zoom: 1.0,
    panX: 0,
    panY: 0,
    rotation: 0,
    adaptationMode: 0,
    bgColor: 'transparent',
    syncTimer: null,

    init: function (canvasId, dotNetRef) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) return;
        this.ctx = this.canvas.getContext('2d', { alpha: true });
        this.dotNetRef = dotNetRef;
        this.width = this.canvas.width;
        this.height = this.canvas.height;

        this.canvas.addEventListener('mousedown', this.onMouseDown.bind(this));
        window.addEventListener('mousemove', this.onMouseMove.bind(this));
        window.addEventListener('mouseup', this.onMouseUp.bind(this));
        this.canvas.addEventListener('wheel', this.onWheel.bind(this), { passive: false });
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

    toggleColorPicker: function() {
        this.isColorPicking = !this.isColorPicking;
        this.canvas.classList.toggle('picking-color', this.isColorPicking);
    },

    rgbToHex: function (r, g, b) {
        return ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
    },

    onMouseDown: function (e) {
        if (this.isColorPicking) {
            const rect = this.canvas.getBoundingClientRect();
            const scaleX = this.canvas.width / rect.width;
            const scaleY = this.canvas.height / rect.height;
            const mouseX = Math.floor((e.clientX - rect.left) * scaleX);
            const mouseY = Math.floor((e.clientY - rect.top) * scaleY);
            
            try {
                const pixel = this.ctx.getImageData(mouseX, mouseY, 1, 1).data;
                // Treat slightly transparent as valid too, just ignore completely transparent if possible, 
                // but actually even transparent could be picked if they want.
                if (pixel[3] > 0) { 
                    const hex = "#" + ("000000" + this.rgbToHex(pixel[0], pixel[1], pixel[2])).slice(-6);
                    if (this.dotNetRef) {
                        this.dotNetRef.invokeMethodAsync('OnColorPicked', hex);
                    }
                }
            } catch(err) { console.error("Color pick error: ", err); }
            
            this.isColorPicking = false;
            this.canvas.classList.remove('picking-color');
            return;
        }

        this.isDragging = true;
        this.dragStartX = e.clientX;
        this.dragStartY = e.clientY;
        this.canvas.classList.add('grabbing');
        document.body.classList.add('global-grabbing');
    },

    onMouseMove: function (e) {
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
