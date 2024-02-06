using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Bullets
{
    public static class BulletManager
    {
        public static readonly float Bullet_Speed = 10f;

        public static List<Bullet> Bullets { get; private set; }

        public static void CreateBullet(Vector2 position, float angleTrajectory)
        {
            Bullets.Add(
                new Bullet
                {
                    Position = position,
                    Angle = angleTrajectory
                }
            );
        }
    }
}
