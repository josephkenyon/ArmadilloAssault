using DilloAssault.Configuration.Textures;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;

namespace DilloAssault.Web.Communication
{
    public abstract class BaseObject
    {
        public TextureName TextureName { get; set; }
        public Point Size { get; set; }
        public Vector2 Position { get; set; }
        public Point SpriteLocation { get; set; }
        public float Angle { get; set; }
        public abstract IDrawableObject GetDrawableObject();
    }
}
