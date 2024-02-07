using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Effects
{
    public class Effect
    {
        public EffectType Type { get; set; }
        public Vector2 Position { get; set; }
        public int FrameCounter { get; set; } = -1;
    }
}
