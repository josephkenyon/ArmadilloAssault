using DilloAssault.Configuration.Textures;
using DilloAssault.Configuration.Weapons;
using DilloAssault.GameState.Battle.Physics;
using DilloAssault.Graphics.Drawing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Crates
{
    public class Crate(CrateType type) : PhysicsObject, IDrawableObject
    {
        private static readonly Random Random = new();

        public Point Size => Grounded ? new Point(64, 64) : new Point(128, 192);
        public TextureName TextureName => Grounded ? TextureName.crates : TextureName.crates_parachuting;
        public CrateType Type { get; private set; } = type;

        public readonly int HealthGiven = type == CrateType.Health ? 35 : 0;
        public readonly WeaponType? WeaponType = type == CrateType.Weapon ? GetRandomWeaponType() : null;

        public override Rectangle GetCollisionBox() => new((int)Position.X + 2, (int)Position.Y + 8, 60, 48);
        public Rectangle? GetSourceRectangle() => new((int)Type * Size.X, 0, Size.X, Size.Y);
        public Rectangle GetDestinationRectangle() => new((int)Position.X - (!Grounded ? 31 : 0), (int)Position.Y - (!Grounded ? 126 : 0), Size.X, Size.Y);
        public List<Rectangle> RelevantCollisionBoxes { get; set; } = [];
        public override Vector2 MaxVelocity => new(8f, 2);

        private static WeaponType GetRandomWeaponType()
        {
            var weaponTypes = Enum.GetValues<WeaponType>();
            var index = Random.NextInt64(1, weaponTypes.Length);

            return weaponTypes[index];
        }
    }
}
