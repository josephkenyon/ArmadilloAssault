using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.GameState.Battle.PowerUps;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Crates
{
    public class CrateManager(ICollection<Rectangle> collisionBoxes, Point sceneSize)
    {
        private List<Rectangle> CollisionBoxes { get; set; } = collisionBoxes.Where(box =>
            {
                var testRectangle = new Rectangle(box.Center.X, box.Top - 10, 48, 10);
                if (collisionBoxes.Any(box => box.Intersects(testRectangle)))
                {
                    return false;
                }

                return box.Top > 48 && box.Width > 96;
            }).ToList();

        public List<Crate> Crates { get; private set; } = [];

        private int TimeSinceLastCrate { get; set; } = 0;
        private int LastX { get; set; } = -1;
        private bool DirectionDown { get; set; }
        private Random Random { get; set; } = new();

        private bool InitialDrop { get; set; } = false;

        private static int CrateSpawnRate(int avatarCount) => 400 - (avatarCount * 70);

        public void UpdateCrates(ICollection<Avatar> avatars)
        {
            var avatarCount = avatars != null ? avatars.Count : 0;

            TimeSinceLastCrate++;

            if (!InitialDrop && TimeSinceLastCrate == CrateSpawnRate(avatarCount))
            {
                InitialDrop = true;
                TimeSinceLastCrate = 0;

                for (var i = 0; i < avatarCount; i++)
                {
                    CreateNewCrate(CrateType.Weapon);
                    CreateNewCrate(CrateType.Weapon);
                    CreateNewCrate(CrateType.Power_Up);
                }
            }
            else if (TimeSinceLastCrate >= CrateSpawnRate(avatarCount) && Crates.Count <= 10)
            {
                TimeSinceLastCrate = 0;
                CreateNewCrate();
            }

            var crates = Crates.Where(crate => !crate.Grounded);
            foreach (var crate in crates)
            {
                UpdateCratePosition(crate);
            }

            if (avatars != null)
            {
                foreach (var avatar in avatars)
                {
                    Crates.RemoveAll(crate =>
                    {
                        if (avatar.GetCollisionBox().Intersects(crate.GetCollisionBox()) && (avatar.CanPickUpPowerUps || crate.Type != CrateType.Power_Up))
                        {
                            GiveCrate(avatar, crate);
                            return true;
                        }

                        return false;
                    });
                }
            }
        }

        private static void UpdateCratePosition(Crate crate)
        {
            if (!crate.Grounded)
            {
                var collisionBox = crate.GetCollisionBox();
                {
                    if (crate.GoingDown)
                    {
                        if (collisionBox.Bottom < crate.FinalY)
                        {
                            crate.SetY(crate.Position.Y + crate.MaxVelocity.Y);
                        }
                        else
                        {
                            crate.Grounded = true;
                        }
                    }
                    else
                    {
                        if (collisionBox.Bottom > crate.FinalY)
                        {
                            crate.SetY(crate.Position.Y - crate.MaxVelocity.Y);
                        }
                        else
                        {
                            crate.Grounded = true;
                        }
                    }
                }
            }
        }

        public void CreateNewCrate(CrateType crateType, int x, int finalY, bool goingDown)
        {
            var crate = new Crate(crateType, null)
            {
                GoingDown = DirectionDown,
            };

            crate.SetX(x);

            if (!goingDown)
            {
                crate.SetY(sceneSize.Y);
            }
            else
            {
                crate.SetY(-200);
            }

            crate.FinalY = finalY;

            crate.GoingDown = goingDown;
        }

        public void CreateNewCrate(CrateType? crateType = null, WeaponType? weaponType = null, Vector2? position = null, bool singleClip = false)
        {
            if (CollisionBoxes.Count == 0)
            {
                return;
            }

            var typeDouble = Random.NextDouble();
            var type = crateType != null ? (CrateType)crateType : CrateType.Weapon;

            if (crateType == null)
            {
                if (typeDouble < 0.65f)
                {
                    type = CrateType.Weapon;
                }
                else if (typeDouble < 0.85f)
                {
                    type = CrateType.Health;
                }
                else
                {
                    type = CrateType.Power_Up;
                }
            }

            Crate crate;

            if (position != null)
            {
                crate = new Crate(type, weaponType, singleClip)
                {
                    GoingDown = DirectionDown,
                };

                var newPosition = (Vector2)position;
                crate.SetX(newPosition.X);
                crate.SetY(newPosition.Y);

                var crateBox = crate.GetCollisionBox();

                var collisionBoxes = CollisionBoxes
                    .Where(box => box.Top > crateBox.Bottom && CollisionHelper.RectanglesIntersectInTheXPlane(crateBox, box))
                    .OrderBy(box => box.Top);

                if (!collisionBoxes.Any())
                {
                    return;
                }

                crate.FinalY = collisionBoxes.First().Top;

                crate.GoingDown = true;
            }
            else
            {
                crate = new Crate(type, weaponType, singleClip)
                {
                    GoingDown = DirectionDown,
                };
            }

            if (position == null)
            {
                int collisionBoxIndex;
                int x;
                Rectangle relevantCollisionBox;
                do
                {
                    collisionBoxIndex = Random.Next(0, CollisionBoxes.Count);
                    relevantCollisionBox = CollisionBoxes[collisionBoxIndex];
                    x = Random.Next(relevantCollisionBox.Left + 24, relevantCollisionBox.Right - 24);
                }
                while (Math.Abs(x - LastX) < 500 && LastX != -1);

                LastX = x;

                crate.SetX(x - (crate.Size.X / 2));

                crate.FinalY = relevantCollisionBox.Top;

                if (!crate.GoingDown)
                {
                    crate.SetY(sceneSize.Y);
                }
                else
                {
                    crate.SetY(-200);
                }

                DirectionDown = !DirectionDown;
            }

            Crates.Add(crate);
        }

        private static void GiveCrate(Avatar avatar, Crate crate)
        {
            if (crate.Type == CrateType.Health)
            {
                avatar.Health += crate.HealthGiven;
            }
            else if (crate.Type == CrateType.Weapon)
            {
                avatar.GiveWeapon((WeaponType)crate.WeaponType, crate.SingleClip);
            }
            else if (crate.Type == CrateType.Power_Up)
            {
                avatar.GivePowerUp((PowerUpType)crate.PowerUpType);
            }

            SoundManager.QueueBattleSound(BattleSound.ammo);
        }

        public CrateFrame GetCrateFrame()
        {
            if (Crates.Count == 0) return null;

            var crateFrame = new CrateFrame();

            foreach (var crate in Crates)
            {
                crateFrame.Types.Add(crate.Type);
                crateFrame.Xs.Add(crate.Position.X);
                crateFrame.Ys.Add(crate.Position.Y);
                crateFrame.Groundeds.Add(crate.Grounded);
                crateFrame.GoingDowns.Add(crate.GoingDown);
            }

            return crateFrame;
        }

        public static ICollection<DrawableCrate> GetDrawableCrates(CrateFrame crateFrame)
        {
            var drawableCrates = new List<DrawableCrate>();

            if (crateFrame == null) return drawableCrates;

            var index = 0;
            foreach (var type in crateFrame.Types)
            {
                try
                {
                    var drawableCrate = new DrawableCrate(
                        type,
                        new Vector2(crateFrame.Xs[index], crateFrame.Ys[index]),
                        crateFrame.Groundeds[index],
                        crateFrame.GoingDowns[index]
                    );

                    drawableCrates.Add(drawableCrate);
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }

                index++;
            }

            return drawableCrates;
        }
    }
}
