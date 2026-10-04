window.gifWall = {
    sentinelObserver: null,
    mediaObserver: null,
    mutationObserver: null,
    resizeObserver: null,
    currentColumnCount: 0,
    isRequestingMore: false,
    observedItems: new WeakSet(),
    activeVideos: new Set(),
    MAX_ACTIVE_VIDEOS: 6,
    refreshTimeout: null,

    calculateColumnCount: function (container) {
        var width = (container && container.clientWidth > 0) ? container.clientWidth : window.innerWidth;
        if (width <= 680) return 2;
        if (width <= 1024) return 3;
        if (width <= 1400) return 4;
        return 5;
    },

    init: function (scrollContainer, sentinelElement, dotNetHelper) {
        this.disconnect();

        if (!sentinelElement || !dotNetHelper) return 4;

        var self = this;
        self.isRequestingMore = false;
        self.observedItems = new WeakSet();
        self.activeVideos = new Set();
        self.currentColumnCount = self.calculateColumnCount(scrollContainer);

        // 1. Sentinel Observer for Infinite Scroll with 350px rootMargin
        this.sentinelObserver = new IntersectionObserver(function (entries) {
            for (var i = 0; i < entries.length; i++) {
                if (entries[i].isIntersecting) {
                    if (!self.isRequestingMore) {
                        self.isRequestingMore = true;
                        dotNetHelper.invokeMethodAsync('OnScrollNearBottom')
                            .then(function () {
                                setTimeout(function () {
                                    self.isRequestingMore = false;
                                }, 350);
                            })
                            .catch(function () {
                                self.isRequestingMore = false;
                            });
                    }
                    break;
                }
            }
        }, {
            root: scrollContainer || null,
            rootMargin: '350px',
            threshold: 0
        });

        this.sentinelObserver.observe(sentinelElement);

        // 2. High-Performance Virtual Playback Optimizer:
        // Automatically plays visible videos and pauses off-screen videos.
        // Pausing is instant and zero-cost during scroll.
        // Only unloads (<video>.load()) the furthest inactive videos when pool exceeds MAX_ACTIVE_VIDEOS (6).
        this.mediaObserver = new IntersectionObserver(function (entries) {
            for (var i = 0; i < entries.length; i++) {
                var entry = entries[i];
                var video = entry.target.querySelector('video');
                if (video) {
                    if (entry.isIntersecting) {
                        self.activateVideo(video);
                    } else {
                        self.pauseVideo(video);
                    }
                }
            }
        }, {
            root: scrollContainer || null,
            rootMargin: '150px 0px 150px 0px',
            threshold: 0
        });

        // 3. Observe initial wall items
        this.refreshItems(scrollContainer);

        // 4. Debounced MutationObserver using requestAnimationFrame
        var wallContainer = scrollContainer ? scrollContainer.querySelector('.gif-masonry-wall') : document.querySelector('.gif-masonry-wall');
        if (wallContainer) {
            this.mutationObserver = new MutationObserver(function () {
                if (self.refreshTimeout) cancelAnimationFrame(self.refreshTimeout);
                self.refreshTimeout = requestAnimationFrame(function () {
                    self.refreshItems(scrollContainer);
                });
            });

            this.mutationObserver.observe(wallContainer, { childList: true, subtree: true });
        }

        // 5. Debounced ResizeObserver for dynamic column responsiveness
        if (window.ResizeObserver && scrollContainer) {
            var resizeTimer = null;
            this.resizeObserver = new ResizeObserver(function (entries) {
                if (!entries || !entries.length) return;
                var width = entries[0].contentRect.width;
                if (width <= 0) return;

                if (resizeTimer) clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    var newCols = self.calculateColumnCount({ clientWidth: width });
                    if (newCols !== self.currentColumnCount) {
                        self.currentColumnCount = newCols;
                        dotNetHelper.invokeMethodAsync('OnColumnCountChanged', newCols).catch(function () {});
                    }
                }, 100);
            });
            this.resizeObserver.observe(scrollContainer);
        }

        return self.currentColumnCount;
    },

    activateVideo: function (video) {
        var dataSrc = video.getAttribute('data-src') || video.getAttribute('src');
        if (!dataSrc) return;

        // Ensure muted & playsinline for seamless hardware decoding
        video.muted = true;
        video.defaultMuted = true;
        video.playsInline = true;

        if (!video.src || video.src === '' || video.src === window.location.href) {
            video.src = dataSrc;
        }

        this.activeVideos.add(video);

        // If pool exceeds safe limit, unload only inactive/paused videos that are farthest away
        if (this.activeVideos.size > this.MAX_ACTIVE_VIDEOS) {
            this.pruneOldestInactiveVideo(video);
        }

        var p = video.play();
        if (p && p.catch) {
            p.catch(function () {});
        }
    },

    pauseVideo: function (video) {
        // Fast, instant pause without blocking the scroll frame
        if (!video.paused) {
            try { video.pause(); } catch (e) {}
        }
    },

    pruneOldestInactiveVideo: function (currentVideo) {
        var iter = this.activeVideos.values();
        var item = iter.next();
        while (!item.done) {
            var v = item.value;
            if (v !== currentVideo && v.paused) {
                this.activeVideos.delete(v);
                try {
                    v.removeAttribute('src');
                    v.load(); // Releases decoder when safe and off-screen
                } catch (e) {}
                break;
            }
            item = iter.next();
        }
    },

    refreshItems: function (scrollContainer) {
        if (!this.mediaObserver) return;
        var wallContainer = scrollContainer ? scrollContainer.querySelector('.gif-masonry-wall') : document.querySelector('.gif-masonry-wall');
        if (!wallContainer) return;

        var items = wallContainer.querySelectorAll('.gif-wall-item');
        for (var i = 0; i < items.length; i++) {
            var it = items[i];
            if (!this.observedItems.has(it)) {
                this.observedItems.add(it);
                this.mediaObserver.observe(it);
            }
        }
    },

    disconnect: function () {
        if (this.refreshTimeout) {
            cancelAnimationFrame(this.refreshTimeout);
            this.refreshTimeout = null;
        }
        if (this.sentinelObserver) {
            this.sentinelObserver.disconnect();
            this.sentinelObserver = null;
        }
        if (this.mediaObserver) {
            this.mediaObserver.disconnect();
            this.mediaObserver = null;
        }
        if (this.mutationObserver) {
            this.mutationObserver.disconnect();
            this.mutationObserver = null;
        }
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
            this.resizeObserver = null;
        }

        // Release all video decoders immediately
        var videos = document.querySelectorAll('.gif-wall-media');
        videos.forEach(function (v) {
            if (v.tagName === 'VIDEO') {
                try {
                    v.pause();
                    v.removeAttribute('src');
                    v.load();
                } catch (e) {}
            }
        });

        if (this.activeVideos) {
            this.activeVideos.clear();
        }
        this.observedItems = new WeakSet();
    }
};
