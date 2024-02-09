using DilloAssault.Configuration;
using DilloAssault.Configuration.Effects;
using DilloAssault.Configuration.Textures;
using DilloAssault.Generics;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace DilloAssault.GameState.Battle.Effects
{
    public class Effect(EffectType type, Vector2 position, Direction? direction = null) : IDrawableObject
    {
        public EffectType Type { get; set; } = type;
        public Vector2 Position { get; set; } = position;
        public int FrameCounter { get; set; }

        private readonly EffectJson Configuration = ConfigurationManager.GetEffectConfiguration(type);
        public TextureName TextureName => Configuration.TextureName;

        public Rectangle GetDestinationRectangle() => new(Position.ToPoint(), Configuration.Size.ToPoint());
        public Rectangle? GetSourceRectangle() => new Rectangle(
            (FrameCounter % Configuration.SpriteRowLength) * Configuration.SpriteSize.X,
            (FrameCounter / Configuration.SpriteRowLength) * Configuration.SpriteSize.Y,
            Configuration.SpriteSize.X,
            Configuration.SpriteSize.Y
        );

        public Direction GetDirection() => direction == Direction.Left ? Direction.Left : Direction.Right;
    }
}
