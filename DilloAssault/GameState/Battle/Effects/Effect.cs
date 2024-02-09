using DilloAssault.Configuration.Effects;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Effects
{
    public class Effect
    {
        public EffectType Type { get; set; }
        public Vector2 Position { get; set; }
        public Direction? Direction { get; set; }
        public int FrameCounter { get; set; }
    }
}
