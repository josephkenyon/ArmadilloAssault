using Microsoft.Xna.Framework;
using System;

namespace DilloAssault.GameState.Battle.Physics
{
    public static class PhysicsHelper
    {
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
    }
}
