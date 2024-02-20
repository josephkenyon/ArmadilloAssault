using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Effects
{
    public class Effect(EffectType type, Vector2 position, Direction? direction = null)
    {
        public EffectType Type { get; set; } = type;
        public Vector2 Position { get; set; } = position;
        public Direction? Direction { get; set; } = direction;
        public int FrameCounter { get; set; } = -1;
    }
}
