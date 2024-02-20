using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Assets
{
    public class Tile : IDrawableObject
    {
        public int Z { get; set; }
        public Point Position { get; set; }
        public Point SpriteLocation { get; set; }
        public TextureName TextureName { get; set; }
        public Rectangle GetDestinationRectangle() => DrawingHelper.GetDestinationRectangle(Position);
        public Rectangle? GetSourceRectangle() => DrawingHelper.GetSourceRectangle(SpriteLocation);
    }
}
