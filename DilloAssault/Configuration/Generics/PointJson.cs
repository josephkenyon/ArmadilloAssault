using Microsoft.Xna.Framework;

namespace DilloAssault.Configuration.Generics
{
    public class PointJson
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point ToPoint() => new(X, Y);

        internal Vector2 ToVector2() => new(X, Y);
    }
}
