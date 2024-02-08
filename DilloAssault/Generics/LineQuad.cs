using Microsoft.Xna.Framework;

namespace DilloAssault.Generics
{
    public class LineQuad()
    {
        public Line Left { get; set; }
        public Line Right { get; set; }
        public Line Top { get; set; }
        public Line Bottom { get; set; }
        public Point Center { get; set; }

        public static LineQuad CreateFrom(Rectangle rectangle)
        {
            return new LineQuad
            {
                Left = new Line(rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height),
                Right = new Line(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                Top = new Line(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y),
                Bottom = new Line(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height)
            };
        }

    }
}
