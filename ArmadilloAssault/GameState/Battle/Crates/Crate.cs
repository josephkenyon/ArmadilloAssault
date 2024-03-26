using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Physics;
using ArmadilloAssault.GameState.Battle.PowerUps;
using Microsoft.Xna.Framework;
using System;

namespace ArmadilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type, WeaponType? weaponType, bool singleClip = false) : PhysicsObject
    {
        private static int NextCrateId = 0;

        private static readonly Random Random = new();

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? weaponType ?? GetRandomWeaponType() : null;
        public readonly PowerUpType? PowerUpType = type == CrateType.Power_Up ? GetRandomPowerUpType() : null;

        public readonly int id = NextCrateId++;
        public Point Size => Grounded ? new Point(96, 96) : new Point(128, 204);
        public CrateType Type { get; private set; } = type;
        public bool SingleClip { get; private set; } = singleClip;
        public int FinalY { get; set; }
        public override Rectangle GetCollisionBox() => new((int)Position.X + 18, (int)Position.Y + 24, 60, 52);
        public override Vector2 MaxVelocity => new(8f, 2);

        public bool GoingDown { get; set; }

        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }

        private static PowerUpType GetRandomPowerUpType()
        {
            var types = Enum.GetValues<PowerUpType>();
            var index = Random.NextInt64(0, types.Length);

            return types[index];
        }
    }
}
