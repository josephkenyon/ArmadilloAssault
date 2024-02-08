using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Effects;
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
             
        public static void CreateBullet(WeaponJson weaponConfiguration, Vector2 position, float angleTrajectory)
        {
            var bullet = new Bullet
            {
                TextureName = weaponConfiguration.BulletTexture,
                Size = weaponConfiguration.BulletSize.ToPoint(),
                Position = position,
                Angle = angleTrajectory
            };

            var frameCounter = 0;
            var bulletPosition = position;
            while(bullet.FrameLife == -1)
            {
                if (CollisionBoxes.Any(box => box.Contains(bulletPosition)) || frameCounter == 200)
                {
                    var newBulletPosition = new Vector2(bulletPosition.X, bulletPosition.Y);
                    while(CollisionBoxes.Any(box => box.Contains(newBulletPosition)) && frameCounter != 200)
                    {
                        newBulletPosition = new Vector2(
                            newBulletPosition.X - (float)(Bullet_Speed / 10 * Math.Cos(bullet.Angle)),
                            newBulletPosition.Y - (float)(Bullet_Speed / 10 * Math.Sin(bullet.Angle))
                        );
                    }

                    bullet.DustCloudOffset = new Vector2(newBulletPosition.X - bulletPosition.X, newBulletPosition.Y - bulletPosition.Y);

                    bullet.FrameLife = frameCounter;
                }

                bulletPosition = GetNewBulletPosition(bulletPosition, bullet.Angle);

                frameCounter++;
            }

            Bullets.Add(bullet);
        }

        public static void UpdateBullets()
        {
            foreach (var bullet in Bullets.Where(bullet => bullet.FrameCounter == bullet.FrameLife))
            {
                EffectManager.CreateEffect(bullet.Position + bullet.DustCloudOffset, EffectType.dust_cloud);
            }

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
