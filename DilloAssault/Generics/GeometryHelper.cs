using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.Generics
{
    public static class GeometryHelper
    {
        public static double DistanceBetweenTwoVectors(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }
    }
}
