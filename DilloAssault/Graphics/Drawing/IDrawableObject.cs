using DilloAssault.Graphics.Drawing.Textures;
using Microsoft.Xna.Framework;

namespace DilloAssault.Graphics.Drawing
{
    public interface IDrawableObject
    {
        int Z { get; }
        Point Position { get; }
        Point SpriteLocation { get; }
        TextureName TextureName { get; }
    }
}
