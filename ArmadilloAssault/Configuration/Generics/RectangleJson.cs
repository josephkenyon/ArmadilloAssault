using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.Configuration.Generics
{
    public class RectangleJson
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static RectangleJson CreateFrom(Rectangle? rectangle) => rectangle != null ? CreateFrom((Rectangle)rectangle) : null;
        public static RectangleJson CreateFrom(Rectangle rectangle) => new() { X = rectangle.X, Y = rectangle.Y, Width = rectangle.Width, Height = rectangle.Height };
        public static RectangleJson CreateFrom(Rectangle rectangle, float scaleFactor) => new() {
            X = (int)(rectangle.X * scaleFactor),
            Y = (int)(rectangle.Y * scaleFactor),
            Width = (int)(rectangle.Width * scaleFactor), 
            Height = (int)(rectangle.Height * scaleFactor)
        };

        internal Rectangle ToRectangle(int fullTileSize = 1)
        {
            return new Rectangle(
                X * fullTileSize,
                Y * fullTileSize,
                Width * fullTileSize,
                Height * fullTileSize
            );
        }
    }
}
