using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Bullets
{
    public static class BulletManager
    {
        public static readonly float Bullet_Speed = 44f;

        public static List<Bullet> Bullets { get; private set; }
        private static ICollection<Rectangle> CollisionBoxes { get; set; }

        public static void Initialize(ICollection<Rectangle> collisionBoxes)
        {
            Bullets = [];
            CollisionBoxes = collisionBoxes;
        }
             
        public static void CreateBullet(Vector2 position, float angleTrajectory)
        {
            var bullet = new Bullet
            {
                Position = position,
                Angle = angleTrajectory
            };

            var frameCounter = 0;
            var bulletPosition = position;
            while(bullet.FrameLife == -1)
            {
                if (CollisionBoxes.Any(box => box.Contains(bulletPosition)) || frameCounter == 200)
                {
                    bullet.FrameLife = frameCounter;
                }

                bulletPosition = GetNewBulletPosition(bulletPosition, bullet.Angle);

                frameCounter++;
            }

            Bullets.Add(bullet);
        }

        public static void UpdateBullets()
        {
            Bullets.RemoveAll(bullet => bullet.FrameCounter == bullet.FrameLife);

            foreach (var bullet in Bullets)
            {
                bullet.Position = GetNewBulletPosition(bullet.Position, bullet.Angle);
                bullet.FrameCounter++;
            }
        }

        private static Vector2 GetNewBulletPosition(Vector2 position, float angle)
        {
            return new Vector2(
                    position.X + (float)(Bullet_Speed * Math.Cos(angle)),
                    position.Y + (float)(Bullet_Speed * Math.Sin(angle))
            );
        }
    }
}
