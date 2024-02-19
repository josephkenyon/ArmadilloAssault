using DilloAssault.Configuration.Effects;
using DilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Effects
{
    public class Effect(EffectType type, Vector2 position, Direction? direction = null)
    {
        public EffectType Type { get; set; } = type;
        public Vector2 Position { get; set; } = position;
        public Direction? Direction { get; set; } = direction;
        public int FrameCounter { get; set; } = -1;
    }
}
