using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Configuration.Generics
{
    public class RectangleJson
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle ToRectangle => new(X, Y, Width, Height);
        public static RectangleJson CreateFrom(Rectangle rectangle) => new() { X = rectangle.X, Y = rectangle.Y, Width = rectangle.Width, Height = rectangle.Height };
    }
}
