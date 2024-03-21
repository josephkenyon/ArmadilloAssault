using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Items;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.Generics;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Graphics.Drawing
{
    public class DrawableItem(ItemType type, Vector2 position, Direction direction, int spriteX, Color? color = null) : IDrawableObject
    {
        public Point Size = new(128, 128);
        public TextureName Texture => ConfigurationManager.GetItemConfiguration(type).TextureName;
        public Rectangle? GetSourceRectangle() => new(spriteX * Size.X, 0, Size.X, Size.Y);
        public Color Color => color ?? Color.White;
        public Direction GetDirection() => direction;
        public Rectangle GetDestinationRectangle() => new(
            (int)position.X - CameraManager.Offset.X,
            (int)position.Y - CameraManager.Offset.Y,
            Size.X, Size.Y
        );
    }
}
