using Microsoft.Xna.Framework;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

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

        public static Point[] RotateRectangle(int x, int y, int width, int height, float angle, float originX, float originY)
        {
            // Calculate the coordinates of the rectangle's corners relative to the origin
            float x1 = x;
            float y1 = y;
            float x2 = x + width;
            float y2 = y;
            float x3 = x + width;
            float y3 = y + height;
            float x4 = x;
            float y4 = y + height;

            // Rotate the coordinates by the given angle
            //var rotatedX1 = (int)(originX + (x1 * Math.Cos(angle) - y1 * Math.Sin(angle)));
            //var rotatedY1 = (int)(originY + (x1 * Math.Sin(angle) + y1 * Math.Cos(angle)));
            //var rotatedX2 = (int)(originX + (x2 * Math.Cos(angle) - y2 * Math.Sin(angle)));
            //var rotatedY2 = (int)(originY + (x2 * Math.Sin(angle) + y2 * Math.Cos(angle)));
            //var rotatedX3 = (int)(originX + (x3 * Math.Cos(angle) - y3 * Math.Sin(angle)));
            //var rotatedY3 = (int)(originY + (x3 * Math.Sin(angle) + y3 * Math.Cos(angle)));
            //var rotatedX4 = (int)(originX + (x4 * Math.Cos(angle) - y4 * Math.Sin(angle)));
            //var rotatedY4 = (int)(originY + (x4 * Math.Sin(angle) + y4 * Math.Cos(angle)));

            var rotated1 = rotate_point(x1, y1, originX, originY, angle);
            var rotated2 = rotate_point(x2, y2, originX, originY, angle);
            var rotated3 = rotate_point(x3, y3, originX, originY, angle);
            var rotated4 = rotate_point(x4, y4, originX, originY, angle);


            // Return the rotated coordinates
            return [rotated1, rotated2, rotated3, rotated4];
        }

        private static Point rotate_point(float pointX, float pointY, float originX, float originY, float angle)
        {
            return new Point(
                (int)(Math.Cos(angle) * (pointX - originX) - Math.Sin(angle) * (pointY - originY) + originX),
                (int)(Math.Sin(angle) * (pointX - originX) + Math.Cos(angle) * (pointY - originY) + originY)
            );
        }

        public static LineQuad CreateFrom(Rectangle rectangle, Vector2 origin, float rotation)
        {
            //double centerX = rectangle.X + rectangle.Width / 2;
            //double centerY = rectangle.Y + rectangle.Height / 2;

            //double translatedOriginX = origin.X - centerX;
            //double translatedOriginY = origin.Y - centerY;

            //double rotatedOriginX = translatedOriginX * Math.Cos(rotation) - translatedOriginY * Math.Sin(rotation);
            //double rotatedOriginY = translatedOriginX * Math.Sin(rotation) + translatedOriginY * Math.Cos(rotation);

            //double finalOriginX = rotatedOriginX + centerX;
            //double finalOriginY = rotatedOriginY + centerY;

            //Point[] corners =
            //[
            //    RotatePoint(rectangle.X, rectangle.Y, rotation, finalOriginX, finalOriginY),
            //    RotatePoint(rectangle.X + rectangle.Width, rectangle.Y, rotation, finalOriginX, finalOriginY),
            //    RotatePoint(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rotation, finalOriginX, finalOriginY),
            //    RotatePoint(rectangle.X, rectangle.Y + rectangle.Height, rotation, finalOriginX, finalOriginY),
            //];

            var corners = RotateRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rotation, origin.X, origin.Y);

            return new LineQuad
            {
                Left = new Line(corners[3].X, corners[3].Y, corners[0].X, corners[0].Y),
                Right = new Line(corners[1].X, corners[1].Y, corners[2].X, corners[2].Y),
                Top = new Line(corners[0].X, corners[0].Y, corners[1].X, corners[1].Y),
                Bottom = new Line(corners[2].X, corners[2].Y, corners[3].X, corners[3].Y)
            };
        }

        private static Point RotatePoint(double x, double y, double angle, double originX, double originY)
        {
            var rotatedX = (int)(originX + (x - originX) * Math.Cos(angle) - (y - originY) * Math.Sin(angle));
            var rotatedY = (int)(originY + (x - originX) * Math.Sin(angle) + (y - originY) * Math.Cos(angle));

            return new Point(rotatedX, rotatedY);
        }
    }
}
