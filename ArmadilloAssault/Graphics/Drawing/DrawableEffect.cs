using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableEffect(EffectType type, Vector2 position, Direction? direction, int frame) : IDrawableObject
    {
        private readonly EffectJson Configuration = ConfigurationManager.GetEffectConfiguration(type);
        public TextureName TextureName => Configuration.TextureName;

        public Rectangle GetDestinationRectangle() => new(position.ToPoint(), Configuration.Size.ToPoint());
        public Rectangle? GetSourceRectangle() => new Rectangle(
            (frame % Configuration.SpriteRowLength) * Configuration.SpriteSize.X,
            (frame / Configuration.SpriteRowLength) * Configuration.SpriteSize.Y,
            Configuration.SpriteSize.X,
            Configuration.SpriteSize.Y
        );

        public Direction GetDirection() => direction == Direction.Left ? Direction.Left : Direction.Right;
    }
}
