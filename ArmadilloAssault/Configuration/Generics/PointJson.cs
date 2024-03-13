using Microsoft.Xna.Framework;

namespace ArmadilloAssault.Configuration.Generics
{
    public class PointJson
    {
        public static PointJson Zero => new(0, 0);

        public PointJson() { }

        public PointJson(int x, int y) {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static PointJson CreateFrom(Point point) => new(point.X, point.Y);

        public Point ToPoint() => new(X, Y);

        internal Vector2 ToVector2() => new(X, Y);
    }
}
