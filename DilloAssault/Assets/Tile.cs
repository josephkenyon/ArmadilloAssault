using DilloAssault.Graphics.Drawing;
using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;

namespace DilloAssault.Assets
{
    public class Tile : IDrawableObject
    {
        public int Z { get; set; }
        public Point Position { get; set; }
        public Point SpriteLocation { get; set; }
        public TextureName TextureName { get; set; }
    }
}
