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
    public class CrateManager(ICollection<Rectangle> collisionBoxes)
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
        private Random Random { get; set; } = new();

        private bool InitialDrop { get; set; } = false;

        private static int CrateSpawnRate(int avatarCount) => 400 - (avatarCount * 70);

        public void UpdateCrates(ICollection<Avatar> avatars)
        {
            if (CollisionBoxes.Count == 0) {
                return;
            }

            TimeSinceLastCrate++;

            if (!InitialDrop && TimeSinceLastCrate == CrateSpawnRate(avatars.Count))
            {
                InitialDrop = true;
                TimeSinceLastCrate = 0;

                foreach(var avatar in avatars)
                {
                    CreateNewCrate(CrateType.Weapon);
                    CreateNewCrate(CrateType.Weapon);
                }

                for (var i = 0; i < avatars.Count; i++)
                {
                    CreateNewCrate(CrateType.Power_Up);
                }
            }
            else if (TimeSinceLastCrate >= CrateSpawnRate(avatars.Count) && Crates.Count <= 10)
            {
                TimeSinceLastCrate = 0;
                CreateNewCrate();
            }

            var crates = Crates.Where(crate => !crate.Grounded);
            foreach (var crate in crates)
            {
                PhysicsManager.ApplyCrateMotion(crate, crate.RelevantCollisionBoxes);
            }

            foreach (var avatar in avatars)
            {
                Crates.RemoveAll(crate =>
                {
                    if (avatar.GetCollisionBox().Intersects(crate.GetCollisionBox()) && ((avatar.CurrentPowerUp == null || avatar.CanPickUpPowerUps) || crate.Type != CrateType.Power_Up)) {
                        GiveCrate(avatar, crate);
                        return true;
                    }
                    return false;
                });
                   
            }
        }

        private void CreateNewCrate(CrateType? crateType = null)
        {
            var typeIndex = Random.NextInt64(0, 4);
            var type = crateType != null ? (CrateType)crateType : CrateType.Weapon;

            if (crateType == null)
            {
                if (typeIndex < 2)
                {
                    type = CrateType.Weapon;
                }
                else if (typeIndex < 3)
                {
                    type = CrateType.Health;
                }
                else
                {
                    type = CrateType.Power_Up;
                }
            }

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

            var crate = new Crate(type)
            {
                RelevantCollisionBoxes = [relevantCollisionBox]
            };

            LastX = x;

            crate.SetX(x - (crate.Size.X / 2));

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
                avatar.GiveWeapon((WeaponType)crate.WeaponType);
            }
            else if (crate.Type == CrateType.Power_Up)
            {
                avatar.GivePowerUp((PowerUpType)crate.PowerUpType);
            }

            SoundManager.QueueBattleSound(BattleSound.ammo);
        }

        public CrateFrame GetCrateFrame()
        {
            var crateFrame = new CrateFrame();

            foreach (var crate in Crates)
            {
                crateFrame.Types.Add(crate.Type);
                crateFrame.PositionXs.Add(crate.Position.X);
                crateFrame.PositionYs.Add(crate.Position.Y);
                crateFrame.Groundeds.Add(crate.Grounded);
            }

            return crateFrame;
        }

        public static ICollection<DrawableCrate> GetDrawableCrates(CrateFrame crateFrame)
        {
            var drawableCrates = new List<DrawableCrate>();

            var index = 0;
            foreach (var type in crateFrame.Types)
            {
                try
                {
                    var drawableCrate = new DrawableCrate(
                        type,
                        new Vector2(crateFrame.PositionXs[index], crateFrame.PositionYs[index]),
                        crateFrame.Groundeds[index]
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
