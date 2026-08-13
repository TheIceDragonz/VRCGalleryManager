window.clipboardInterop = {
    readImageAsBase64: async function () {
        try {
            const clipboardItems = await navigator.clipboard.read();
            for (const clipboardItem of clipboardItems) {
                const imageTypes = clipboardItem.types.filter(type => type.startsWith('image/'));
                if (imageTypes.length > 0) {
                    const blob = await clipboardItem.getType(imageTypes[0]);
                    return new Promise((resolve, reject) => {
                        const reader = new FileReader();
                        reader.onloadend = () => resolve(reader.result);
                        reader.onerror = reject;
                        reader.readAsDataURL(blob);
                    });
                }
            }
            return null; // No image found
        } catch (error) {
            console.error("Failed to read from clipboard:", error);
            return null;
        }
    },
    writeImageFromUrl: async function (url) {
        try {
            const response = await fetch(url);
            const blob = await response.blob();
            await navigator.clipboard.write([
                new ClipboardItem({
                    [blob.type]: blob
                })
            ]);
            return true;
        } catch (error) {
            console.error("Failed to copy image to clipboard:", error);
            return false;
        }
    },
    writeText: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (error) {
            console.error("Failed to copy text to clipboard:", error);
            return false;
        }
    }
};
