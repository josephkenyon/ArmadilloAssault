using ArmadilloAssault.Configuration.Generics;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.Generics
{
    public static class MathUtils
    {
        public static double DistanceBetweenTwoVectors(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        public static bool FloatsAreEqual(float a, float b)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);

            float diff = Math.Abs(absA - absB);

            if (a == b)
            {
                return true;
            }
            else if (diff < 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetAlpha(ColorJson colorJson, int avatarIndex, int playerIndex)
        {
            if (colorJson.A != 255)
            {
                colorJson.A = avatarIndex == playerIndex ? 50 : 2;
            }

            return colorJson.A;
        }
    }
}
