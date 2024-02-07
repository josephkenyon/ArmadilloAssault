using DilloAssault.Configuration.Generics;
using DilloAssault.Graphics.Drawing.Textures;

namespace DilloAssault.Configuration.Effects
{
    public class EffectJson
    {
        public string Type { get; set; }
        public TextureName TextureName { get; set; }
        public PointJson Size { get; set; }
        public PointJson SpriteSize { get; set; }
        public int FrameLife { get; set; }
        public int SpriteRowLength { get; set; }
    }
}
