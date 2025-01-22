using VRCGalleryManager.Properties;

namespace VRCGalleryManager.Core
{
    internal class EmojiType
    {
        public class TypeWithImage
        {
            public string Type { get; set; }
            public Image Image { get; set; }

            public TypeWithImage(string type, Image image)
            {
                Type = type;
                Image = image;
            }
        }

        public List<TypeWithImage> TypesWithImages { get; } = new List<TypeWithImage>
        {
            new TypeWithImage("Aura", Resources.Preview_B2_Aura),
            new TypeWithImage("Bats", Resources.Preview_B2_Fall_Bats),
            new TypeWithImage("Bees", Resources.Preview_B2_Bees),
            new TypeWithImage("Bounce", Resources.Preview_B2_Bounce),
            new TypeWithImage("Cloud", Resources.Preview_B2_Cloud),
            new TypeWithImage("Confetti", Resources.Preview_B2_Winter_Confetti),
            new TypeWithImage("Crying", Resources.Preview_B2_Crying),
            new TypeWithImage("Dislike", Resources.Preview_B2_Dislike),
            new TypeWithImage("Fire", Resources.Preview_B2_Fire),
            new TypeWithImage("Idea", Resources.Preview_B2_Idea),
            new TypeWithImage("Lasers", Resources.Preview_B2_Lasers),
            new TypeWithImage("Like", Resources.Preview_B2_Like),
            new TypeWithImage("Magnet", Resources.Preview_B2_Magnet),
            new TypeWithImage("Mistletoe", Resources.Preview_B2_Winter_Mistletoe),
            new TypeWithImage("Money", Resources.Preview_B2_Money),
            new TypeWithImage("Noise", Resources.Preview_B2_Noise),
            new TypeWithImage("Orbit", Resources.Preview_B2_Orbit),
            new TypeWithImage("Pizza", Resources.Preview_B2_Pizza),
            new TypeWithImage("Rain", Resources.Preview_B2_Rain),
            new TypeWithImage("Rotate", Resources.Preview_B2_Rotate),
            new TypeWithImage("Shake", Resources.Preview_B2_Shake),
            new TypeWithImage("Snow", Resources.Preview_B2_Spin),
            new TypeWithImage("Snowball", Resources.Preview_B2_Winter_Snowball),
            new TypeWithImage("Spin", Resources.Preview_B2_Spin),
            new TypeWithImage("Splash", Resources.Preview_B2_SummerSplash),
            new TypeWithImage("Stop", Resources.Preview_B2_Stop),
            new TypeWithImage("ZZZ", Resources.Preview_B2_ZZZ)
        };
    }
}
