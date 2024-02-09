using DilloAssault.Configuration.Textures;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace DilloAssault.Assets
{
    public class Tile : IDrawableObject
    {
        public int Z { get; set; }
        public Point Position { get; set; }
        public Point SpriteLocation { get; set; }
        public TextureName TextureName { get; set; }

        Vector2 IDrawableObject.Position => Position.ToVector2();

        public Rectangle GetDestinationRectangle() => DrawingHelper.GetDestinationRectangle(Position);
        public Rectangle? GetSourceRectangle() => DrawingHelper.GetSourceRectangle(SpriteLocation);
    }
}
