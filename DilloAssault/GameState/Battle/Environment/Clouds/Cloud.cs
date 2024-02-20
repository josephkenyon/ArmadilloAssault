using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Environment.Clouds
{
    public class Cloud : IDrawableObject
    {
        public Vector2 Position { get; set; }
        public Point Size { get; set; }
        public TextureName TextureName => TextureName.clouds;
        public Point SpriteLocation { get; set; }
        public float Speed { get; set; }
        public bool Foreground { get; set; }

        public Rectangle? GetSourceRectangle() => new Rectangle(
            CloudManager.CloudSpriteSize * SpriteLocation.X,
            CloudManager.CloudSpriteSize * SpriteLocation.Y,
            CloudManager.CloudSpriteSize,
            CloudManager.CloudSpriteSize
        );

        public float Opacity => Foreground ? Math.Clamp(0.25f * Math.Abs(Speed), 0.1f, 0.3f) : Math.Clamp(0.45f * Math.Abs(Speed), 0.2f, 1f);

        public Rectangle GetDestinationRectangle() => new(Position.ToPoint(), Size);
    }
}
