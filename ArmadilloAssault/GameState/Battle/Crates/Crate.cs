using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Configuration.Weapons;
using ArmadilloAssault.GameState.Battle.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type) : PhysicsObject
    {
        private static readonly Random Random = new();

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? GetRandomWeaponType() : null;

        public Point Size => Grounded ? new Point(64, 64) : new Point(128, 192);
        public TextureName TextureName => Grounded ? TextureName.crates : TextureName.crates_parachuting;
        public CrateType Type { get; private set; } = type;
        public List<Rectangle> RelevantCollisionBoxes { get; set; } = [];
        public override Rectangle GetCollisionBox() => new((int)Position.X + 2, (int)Position.Y + 8, 60, 48);
        public override Vector2 MaxVelocity => new(8f, 2);

        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }
    }
}
