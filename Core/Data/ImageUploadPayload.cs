namespace VRCGalleryManager.Core.DTO
{
    public enum TagType
    {
        Icon,
        Gallery,
        Emoji,
        EmojiAnimated,
        Sticker,
        Print
    }

    public class ImageUploadPayload
    {
        public string FilePath { get; set; }
        public TagType? Tag { get; set; }
        public string? MaskTag { get; set; }
        public string? MaskType { get; set; }
        public string? AnimationStyle { get; set; }
        public int? Frames { get; set; }
        public int? FramesOverTime { get; set; }

        public ImageUploadPayload(string filePath)
        {
            FilePath = filePath;
        }
    }
}
