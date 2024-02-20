
using Microsoft.Xna.Framework;

namespace ArmadilloAssault.GameState.Battle.Physics
{
    public static class CollisionHelper
    {
        public static readonly int PassableYThreshold = 18;
        public static Rectangle OffsetRectangle(Rectangle rectangle, Vector2 Offset) => new(rectangle.X + (int)Offset.X, rectangle.Y + (int)Offset.Y, rectangle.Width, rectangle.Height);
        public static Rectangle FlipRectangle(Rectangle rectangle, int spriteWidth) => new(spriteWidth - rectangle.X - rectangle.Width, rectangle.Y, rectangle.Width, rectangle.Height);

        public static bool RectanglesIntersectInTheXPlane(Rectangle rectangleA, Rectangle rectangleB)
        {
            var newA = new Rectangle(rectangleA.X, 0, rectangleA.Width, 10);
            var newB = new Rectangle(rectangleB.X, 0, rectangleB.Width, 10);

            return newA.Intersects(newB);
        }

        public static bool RectanglesIntersectInTheYPlane(Rectangle rectangleA, Rectangle rectangleB)
        {
            var newA = new Rectangle(0, rectangleA.Y, 10, rectangleA.Height);
            var newB = new Rectangle(0, rectangleB.Y, 10, rectangleB.Height);

            return newA.Intersects(newB);
        }
    }
}
