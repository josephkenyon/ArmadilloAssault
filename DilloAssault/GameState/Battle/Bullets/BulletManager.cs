using ArmadilloAssault.Configuration.Effects;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Avatars;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Bullets
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
                Position = position,
                Angle = angleTrajectory,
                WeaponType = weaponConfiguration.Type
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
                                    SoundManager.QueueBattleSound(BattleSound.ricochet);
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

        public static BulletFrame GetBulletFrame()
        {
            var bulletFrame = new BulletFrame();

            foreach (var bullet in Bullets)
            {
                bulletFrame.WeaponTypes.Add(bullet.WeaponType);
                bulletFrame.PositionXs.Add(bullet.Position.X);
                bulletFrame.PositionYs.Add(bullet.Position.Y);
                bulletFrame.Rotations.Add(bullet.Angle);
            }

            return bulletFrame;
        }

        public static ICollection<DrawableBullet> GetDrawableBullets(BulletFrame bulletFrame)
        {
            var drawableBullets = new List<DrawableBullet>();

            var index = 0;
            foreach (var type in bulletFrame.WeaponTypes)
            {
                try
                {
                    var drawableBullet = new DrawableBullet(
                        type,
                        new Vector2(bulletFrame.PositionXs[index], bulletFrame.PositionYs[index]),
                        bulletFrame.Rotations[index]
                    );

                    drawableBullets.Add(drawableBullet);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }

                index++;
            }

            return drawableBullets;
        }

        //private class ContactPointDetails(bool reflect, Vector2? contactPoint, Line edge)
        //{
        //    public bool Reflect { get; set; }
        //    public Vector2? ContactPoint { get; set; }
        //    public Line Edge { get; set; }
        //}
    }
}
