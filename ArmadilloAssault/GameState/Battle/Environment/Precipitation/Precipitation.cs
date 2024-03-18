using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Environment.Precipitation
{
    public class PrecipitationParticle(PrecipitationType type) : IDrawableObject
    {
        public Vector2 Position { get; set; }
        public Point Size { get; set; }
        public TextureName Texture => type == PrecipitationType.Snow ? TextureName.snowball : TextureName.rain;
        public float Rotation { get; set; }

        public bool Foreground { get; set; }

        public float Opacity => Foreground ? 0.9f : 0.75f;

        public Rectangle GetDestinationRectangle() => new(Position.ToPoint() - CameraManager.Offset, Size);
    }
}
