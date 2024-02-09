using DilloAssault.Configuration.Generics;
using DilloAssault.Configuration.Textures;

namespace DilloAssault.Configuration.Effects
{
    public class EffectJson
    {
        public EffectType Type { get; set; }
        public TextureName TextureName { get; set; }
        public PointJson Size { get; set; }
        public PointJson SpriteSize { get; set; }
        public int FrameLife { get; set; }
        public int SpriteRowLength { get; set; }
    }
}
