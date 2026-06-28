window.imageEditor = {
    canvas: null,
    ctx: null,
    image: null,
    zoomFactor: 1.0,
    panX: 0,
    panY: 0,
    isDragging: false,
    dragStartX: 0,
    dragStartY: 0,
    removeBgEnabled: false,
    removeBgColor: { r: 255, g: 255, b: 255 },
    removeBgTolerance: 15,
    dotNetRef: null,

    init: function (canvasId, dotNetRef) {
        this.canvas = document.getElementById(canvasId);
        this.ctx = this.canvas.getContext('2d');
        this.dotNetRef = dotNetRef;

        this.canvas.addEventListener('mousedown', this.onMouseDown.bind(this));
        this.canvas.addEventListener('mousemove', this.onMouseMove.bind(this));
        this.canvas.addEventListener('mouseup', this.onMouseUp.bind(this));
        this.canvas.addEventListener('wheel', this.onWheel.bind(this));
    },

    loadImage: function (base64Image) {
        this.image = new Image();
        this.image.onload = () => {
            this.panX = (this.canvas.width - this.image.width) / 2;
            this.panY = (this.canvas.height - this.image.height) / 2;
            this.zoomFactor = 1.0;
            this.draw();
        };
        this.image.src = base64Image;
    },

    draw: function () {
        if (!this.ctx || !this.image) return;

        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

        // draw checkerboard background
        let size = 20;
        for (let i = 0; i < this.canvas.width; i += size) {
            for (let j = 0; j < this.canvas.height; j += size) {
                this.ctx.fillStyle = (i / size) % 2 === (j / size) % 2 ? '#ccc' : '#eee';
                this.ctx.fillRect(i, j, size, size);
            }
        }

        this.ctx.save();
        this.ctx.translate(this.canvas.width / 2, this.canvas.height / 2);
        this.ctx.scale(this.zoomFactor, this.zoomFactor);
        this.ctx.translate(-this.canvas.width / 2, -this.canvas.height / 2);

        if (this.removeBgEnabled) {
            // we have to draw it to a hidden canvas, process pixels, then draw that
            let offscreen = document.createElement('canvas');
            offscreen.width = this.image.width;
            offscreen.height = this.image.height;
            let octx = offscreen.getContext('2d');
            octx.drawImage(this.image, 0, 0);

            let imgData = octx.getImageData(0, 0, offscreen.width, offscreen.height);
            let data = imgData.data;

            for (let i = 0; i < data.length; i += 4) {
                let r = data[i];
                let g = data[i + 1];
                let b = data[i + 2];

                if (Math.abs(r - this.removeBgColor.r) <= this.removeBgTolerance &&
                    Math.abs(g - this.removeBgColor.g) <= this.removeBgTolerance &&
                    Math.abs(b - this.removeBgColor.b) <= this.removeBgTolerance) {
                    data[i + 3] = 0; // set alpha to 0
                }
            }
            octx.putImageData(imgData, 0, 0);
            this.ctx.drawImage(offscreen, this.panX, this.panY);
        } else {
            this.ctx.drawImage(this.image, this.panX, this.panY);
        }

        this.ctx.restore();
    },

    onMouseDown: function (e) {
        this.isDragging = true;
        this.dragStartX = e.clientX - this.panX * this.zoomFactor;
        this.dragStartY = e.clientY - this.panY * this.zoomFactor;
    },

    onMouseMove: function (e) {
        if (!this.isDragging) return;
        this.panX = (e.clientX - this.dragStartX) / this.zoomFactor;
        this.panY = (e.clientY - this.dragStartY) / this.zoomFactor;
        this.draw();
    },

    onMouseUp: function () {
        this.isDragging = false;
    },

    onWheel: function (e) {
        e.preventDefault();
        if (e.deltaY < 0) {
            this.zoomFactor *= 1.1;
        } else {
            this.zoomFactor *= 0.9;
        }
        this.draw();
    },

    setRemoveBg: function (enabled, r, g, b, tolerance) {
        this.removeBgEnabled = enabled;
        this.removeBgColor = { r: r, g: g, b: b };
        this.removeBgTolerance = tolerance;
        this.draw();
    },
    
    getImageDataAsBase64: function () {
        // Return only the cropped/edited part inside the canvas
        return this.canvas.toDataURL("image/png");
    }
};
