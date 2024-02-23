using ArmadilloAssault.Configuration.Generics;
using ArmadilloAssault.Configuration.Textures;

namespace ArmadilloAssault.Configuration.Effects
{
    public class EffectJson
    {
        public EffectType Type { get; set; }
        public TextureName TextureName { get; set; }
        public PointJson Size { get; set; }
        public PointJson SpriteSize { get; set; }
        public int FrameSkip { get; set; } = 1;
        public int FrameLife { get; set; }
        public int SpriteRowLength { get; set; }
    }
}
