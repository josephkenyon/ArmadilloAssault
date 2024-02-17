using DilloAssault.Configuration.Effects;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Effects;
using DilloAssault.Generics;
using DilloAssault.Web.Communication.Updates;
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
        private static List<KeyValuePair<Rectangle, LineQuad>> CollisionLinePairs { get; set; }

        public static void Initialize(ICollection<Rectangle> collisionBoxes)
        {
            Bullets = [];
            CollisionLinePairs = collisionBoxes.Select(box => new KeyValuePair<Rectangle, LineQuad>(box, LineQuad.CreateFrom(box))).ToList();
        }
             
        public static void CreateBullet(WeaponJson weaponConfiguration, Vector2 position, float angleTrajectory)
        {
            var bullet = new Bullet
            {
                TextureName = weaponConfiguration.BulletTexture,
                Size = weaponConfiguration.BulletSize.ToPoint(),
                Position = position,
                Angle = angleTrajectory,
                Damage = weaponConfiguration.BulletDamage
            };

            Bullets.Add(bullet);
        }

        public static void UpdateBullets(List<Avatar> avatars)
        {
            var boxLists = avatars.Select(avatar => {
                var avatarHurtBoxes = avatar.GetHurtBoxes().OrderBy(rec => rec.Top);


                var headBox = true && !avatar.IsSpinning;

                var hurtBoxes = avatarHurtBoxes.Select(box =>
                {
                    var keyValuePair = new KeyValuePair<HurtBoxType, LineQuad>(
                        headBox ? HurtBoxType.Head : HurtBoxType.Standard,
                        avatar.IsSpinning ? LineQuad.CreateFrom(box, avatar.OffsetOrigin, avatar.Rotation) : LineQuad.CreateFrom(box)
                    );

                    headBox = false;

                    return keyValuePair;
                }
                ).ToList();

                var shellBox = avatar.GetShellBox();
                hurtBoxes.Add(new(HurtBoxType.Shell, avatar.IsSpinning ? LineQuad.CreateFrom(shellBox, avatar.OffsetOrigin, avatar.Rotation) : LineQuad.CreateFrom(shellBox)));

                return hurtBoxes;
            }).ToList();

            Bullets.RemoveAll(bullet => {
                var endOfLife = false;
        
                var newBulletPosition = GetNewBulletPosition(bullet.Position, bullet.Angle);

                var bulletTrajectory = new Line(bullet.Position, newBulletPosition);

                var terrainIntersections = CollisionLinePairs
                    .Select(linePair => bulletTrajectory.GetIntersection(linePair.Value))
                    .Where(vector => vector != null)
                    .Select(vector => (Vector2) vector)
                    .OrderBy(vector => MathUtils.DistanceBetweenTwoVectors(bullet.Position, vector));

                if (terrainIntersections.Any())
                {
                    var terrain = terrainIntersections.First();
                    var testBulletPosition = bullet.Position;

                    EffectManager.CreateEffect(terrainIntersections.First(), EffectType.dust_cloud);
                    endOfLife = true;
                }

                if (!endOfLife)
                {
                    for (int i = 0; i < avatars.Count; i++)
                    {
                        var avatarCenter = avatars[i].GetCenter();
                        if (MathUtils.DistanceBetweenTwoVectors(avatarCenter, bullet.Position) <= (Bullet_Speed * 2))
                        {
                            var intersections = boxLists[i]
                                .Select(linePair => new KeyValuePair<HurtBoxType, Vector2?>(linePair.Key, bulletTrajectory.GetIntersection(linePair.Value)))
                                .Where(pair => pair.Value != null)
                                .Select(pair => new KeyValuePair<HurtBoxType, Vector2>(pair.Key, (Vector2)pair.Value))
                                .OrderBy(pair => MathUtils.DistanceBetweenTwoVectors(bullet.Position, pair.Value));

                            if (intersections.Any())
                            {
                                var pair = intersections.First();
                                if (pair.Key == HurtBoxType.Shell)
                                {
                                    RichochetBullet(bullet);
                                    EffectManager.CreateEffect(pair.Value, EffectType.ricochet);
                                }
                                else
                                {
                                    avatars[i].HitByBullet(bullet, pair.Key == HurtBoxType.Head);
                                    EffectManager.CreateEffect(pair.Value, EffectType.blood_splatter);
                                    endOfLife = true;
                                }
                            }
                        }
                    }
                }

                if (!endOfLife)
                {
                    bullet.Position = GetNewBulletPosition(bullet.Position, bullet.Angle);
                }

                return endOfLife;
            });
        }

        private static Vector2 GetNewBulletPosition(Vector2 position, float angle)
        {
            return new Vector2(
                    position.X + (float)(Bullet_Speed * Math.Cos(angle)),
                    position.Y + (float)(Bullet_Speed * Math.Sin(angle))
            );
        }

        private static void RichochetBullet(Bullet bullet)
        {
            var goingUp = Math.Sin(bullet.Angle) < 0;

            var ricochetAmount = (float)(Math.PI / 2f);

            bullet.Angle += goingUp ? ricochetAmount : -ricochetAmount;
        }

        private static double CalculateNormalAngle(double x1, double y1, double x2, double y2)
        {
            // Calculate the normal vector
            var normalVector = CalculateNormalVector(x1, y1, x2, y2);

            // Calculate the angle of the normal vector using atan2
            double angle = Math.Atan2(normalVector.Item2, normalVector.Item1);

            return angle;
        }

        private static (double, double) CalculateNormalVector(double x1, double y1, double x2, double y2)
        {
            // Calculate the direction vector
            double directionX = x2 - x1;
            double directionY = y2 - y1;

            // Calculate the length of the direction vector
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);

            // Normalize the direction vector
            double normalizedDirectionX = directionX / length;
            double normalizedDirectionY = directionY / length;

            // Rotate the normalized direction vector by 90 degrees to get the normal vector
            double normalX = -normalizedDirectionY;
            double normalY = normalizedDirectionX;

            return (normalX, normalY);
        }

        public static BulletsUpdate GetBulletsUpdate()
        {
            var bulletUpdates = new BulletsUpdate();

            foreach (var bullet in Bullets)
            {
                bulletUpdates.Textures.Add(bullet.TextureName);
                bulletUpdates.Xs.Add((int)bullet.Position.X);
                bulletUpdates.Ys.Add((int)bullet.Position.Y);
                bulletUpdates.SizeXs.Add(bullet.Size.X);
                bulletUpdates.SizeYs.Add(bullet.Size.Y);
                bulletUpdates.Rotations.Add((int)bullet.GetRotation());
            }

            return bulletUpdates;
        }

        public static void UpdateBullets(BulletsUpdate bulletsUpdate)
        {
            Bullets = [];

            for (int i = 0; i < bulletsUpdate.Textures.Count; i++)
            {
                var Bullet = new Bullet
                {
                    Angle = bulletsUpdate.Rotations[i],
                    Position = bulletsUpdate.GetPosition(i),
                    TextureName = bulletsUpdate.Textures[i],
                    Size = new Point(bulletsUpdate.SizeXs[i], bulletsUpdate.SizeYs[i])
                };

                Bullets.Add(Bullet);
            }
        }

        //private class ContactPointDetails(bool reflect, Vector2? contactPoint, Line edge)
        //{
        //    public bool Reflect { get; set; }
        //    public Vector2? ContactPoint { get; set; }
        //    public Line Edge { get; set; }
        //}
    }
}
