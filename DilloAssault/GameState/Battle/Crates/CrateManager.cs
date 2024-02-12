using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Avatars;
using DilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.GameState.Battle.Crates
{
    public static class CrateManager
    {
        private static List<Rectangle> CollisionBoxes { get; set; }
        public static List<Crate> Crates { get; private set; }

        private static int TimeSinceLastCrate { get; set; }
        private static Random Random { get; set; }

        private static bool InitialDrop { get; set; } = false;

        public static void Initialize(ICollection<Rectangle> collisionBoxes)
        {
            CollisionBoxes = collisionBoxes.Where(box =>
            {
                var testRectangle = new Rectangle(box.Center.X, box.Top - 10, 48, 10);
                if (collisionBoxes.Any(box => box.Intersects(testRectangle))) {
                    return false;
                }

                return box.Top > 48;
            }).ToList();

            Crates = [];

            Random = new();
        }

        public static void UpdateCrates(ICollection<Avatar> avatars)
        {
            TimeSinceLastCrate++;

            if (!InitialDrop && TimeSinceLastCrate == 60 * 5)
            {
                InitialDrop = true;
                TimeSinceLastCrate = 0;

                foreach(var avatar in avatars)
                {
                    CreateNewCrate(CrateType.Weapon);
                    CreateNewCrate(CrateType.Weapon);
                    CreateNewCrate(CrateType.Weapon);
                }
            }
            else if (TimeSinceLastCrate >= 60 * 10 && Crates.Count <= 10)
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
            var type = (crateType != null) ? (int)crateType : Random.Next(0, 2);

            var relevantCollisionBox = CollisionBoxes[collisionBoxIndex];

            var crate = new Crate((CrateType)type)
            {
                RelevantCollisionBoxes = [relevantCollisionBox]
            };

            var x = Random.Next(relevantCollisionBox.Left + 24, relevantCollisionBox.Right - 24);

            crate.SetX(x - (Crate.Size / 2));

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
        }
    }
}
