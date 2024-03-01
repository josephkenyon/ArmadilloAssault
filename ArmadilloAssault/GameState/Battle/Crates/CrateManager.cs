using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Physics;
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
    public static class CrateManager
    {
        private static List<Rectangle> CollisionBoxes { get; set; }
        public static List<Crate> Crates { get; private set; }

        private static int TimeSinceLastCrate { get; set; }
        private static Random Random { get; set; }

        private static bool InitialDrop { get; set; }

        private static int CrateSpawnRate(int avatarCount) => 100 - (avatarCount * 50);

        public static void Initialize(ICollection<Rectangle> collisionBoxes)
        {
            InitialDrop = false;

            TimeSinceLastCrate = 0;

            CollisionBoxes = collisionBoxes.Where(box =>
            {
                var testRectangle = new Rectangle(box.Center.X, box.Top - 10, 48, 10);
                if (collisionBoxes.Any(box => box.Intersects(testRectangle))) {
                    return false;
                }

                return box.Top > 48 && box.Width > 96;
            }).ToList();

            Crates = [];

            Random = new();
        }

        public static void UpdateCrates(ICollection<Avatar> avatars)
        {
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
            }
            else if (TimeSinceLastCrate >= CrateSpawnRate(avatars.Count) && Crates.Count <= 10)
            {
                TimeSinceLastCrate = 0;
                CreateNewCrate();
            }

            var crates = Crates.Where(crate => !crate.Grounded);
            foreach (var crate in crates)
            {
                PhysicsManager.Update(crate, crate.RelevantCollisionBoxes);
            }

            foreach (var avatar in avatars)
            {
                Crates.RemoveAll(crate =>
                {
                    if (avatar.GetCollisionBox().Intersects(crate.GetCollisionBox())) {
                        GiveCrate(avatar, crate);
                        return true;
                    }
                    return false;
                });
                   
            }
        }

        private static void CreateNewCrate(CrateType? crateType = null)
        {
            var collisionBoxIndex = Random.Next(0, CollisionBoxes.Count);
            var type = (crateType != null) ? (int)crateType : Math.Clamp(Random.Next(-1, 3), 0, 2);

            var relevantCollisionBox = CollisionBoxes[collisionBoxIndex];

            var crate = new Crate((CrateType)type)
            {
                RelevantCollisionBoxes = [relevantCollisionBox]
            };

            var x = Random.Next(relevantCollisionBox.Left + 24, relevantCollisionBox.Right - 24);

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

            SoundManager.QueueBattleSound(BattleSound.ammo);
        }

        public static CrateFrame GetCrateFrame()
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
