using VRChat.API.Client;
using VRChat.API.Model;

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

    public enum FileType
    {
        Print,
        NonPrint
    }

    public class ImageUploadPayload
    {
        public FileType FileType { get; set; }
        public FileParameter FilePath { get; set; }
        public ImagePurpose Tag { get; set; }
        public ImageMask MaskTag { get; set; }
        public ImageMask MaskType { get; set; }
        public ImageAnimationStyle AnimationStyle { get; set; }
        public int? Frames { get; set; }
        public int? FramesOverTime { get; set; }
        public string? Note { get; set; }

        public ImageLoopStyle ImageLoopStyle { get; set; }

        public ImageUploadPayload(FileParameter filePath)
        {
            FilePath = filePath;
            FileType = FileType.NonPrint;
        }
    }
}
